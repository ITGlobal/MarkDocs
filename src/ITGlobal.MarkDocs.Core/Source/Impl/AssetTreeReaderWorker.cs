using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Impl;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class AssetTreeReaderWorker
    {
        #region fields

        private readonly IFormat _format;
        private readonly IContentHashProvider _contentHashProvider;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IContentMetadataProvider _contentMetadataProvider;
        private readonly string[] _includeFiles;
        private readonly string[] _indexFileNames;
        private readonly string[] _ignorePatterns;

        private readonly ISourceTreeRoot _root;
        private readonly ICompilationReportBuilder _report;

        #endregion

        #region .ctor

        public AssetTreeReaderWorker(
            IFormat format,
            IContentHashProvider contentHashProvider,
            IContentTypeProvider contentTypeProvider,
            IContentMetadataProvider contentMetadataProvider,
            string[] includeFiles,
            string[] indexFileNames,
            string[] ignorePatterns,
            ISourceTreeRoot root,
            ICompilationReportBuilder report)
        {
            _format = format;
            _contentHashProvider = contentHashProvider;
            _contentTypeProvider = contentTypeProvider;
            _contentMetadataProvider = contentMetadataProvider;
            _includeFiles = includeFiles;
            _indexFileNames = indexFileNames;
            _ignorePatterns = ignorePatterns;
            _root = root;
            _report = report;
        }

        #endregion

        #region metadata

        private string RootDirectory => _root.RootDirectory;

        #endregion

        #region public methods

        public AssetTree ReadAssetTree()
        {
            var consumedFiles = new HashSet<string>();

            var rootPage = BranchPage(RootDirectory, consumedFiles);
            WalkPages(rootPage, page => consumedFiles.Add(page.AbsolutePath));
            var attachments = CollectAttachments(consumedFiles);

            var tree = new AssetTree(_root.SourceTree.Id, _root.SourceInfo, rootPage, attachments);

            return tree;

            void WalkPages(PageAsset asset, Action<PageAsset> action)
            {
                action(asset);
                if (asset is BranchPageAsset branchPage)
                {
                    foreach (var subpage in branchPage.Subpages)
                    {
                        WalkPages(subpage, action);
                    }
                }
            }
        }

        #endregion

        #region private methods

        private bool ScanDirectory(
            string directory,
            HashSet<string> consumedFiles,
            out PageMetadata metadata,
            out string indexPageFileName,
            out List<PageAsset> pages)
        {
            var ignoreRules = GetIgnoreRulesForDirectory(directory);

            pages = new List<PageAsset>();
            foreach (var subDirectory in Directory.EnumerateDirectories(directory))
            {
                if (ignoreRules.ShouldIgnore(subDirectory))
                {
                    continue;
                }

                var page = BranchPage(subDirectory, consumedFiles);
                if (page != null)
                {
                    pages.Add(page);
                }
            }

            var pageFiles = ScanDirectoryFiles(directory, out indexPageFileName);
            foreach (var filename in pageFiles)
            {
                pages.Add(LeafPage(filename, consumedFiles));
            }

            metadata = null;
            if (indexPageFileName != null)
            {
                metadata = GetMetadata(indexPageFileName, consumedFiles, true);
            }
            metadata = metadata ?? PageMetadata.Empty;
            if (string.IsNullOrEmpty(metadata.Title))
            {
                metadata = metadata.WithTitle(Path.GetFileNameWithoutExtension(directory));
            }

            if (pages.Count == 0)
            {
                if (indexPageFileName == null)
                {
                    return false;
                }
            }

            return true;
        }

        private List<string> ScanDirectoryFiles(string directory, out string indexPageFileName)
        {
            var ignoreRules = GetIgnoreRulesForDirectory(directory);

            var files = from filter in _includeFiles
                        from filename in Directory.EnumerateFiles(directory, filter)
                        select filename;

            var pages = new List<string>();
            var possibleIndexPages = new Dictionary<int, string>();

            foreach (var filename in files)
            {
                if (ignoreRules.ShouldIgnore(filename))
                {
                    continue;
                }

                var name = Path.GetFileName(filename);

                var matchesIndexFileName = false;
                for (var i = 0; i < _indexFileNames.Length; i++)
                {
                    if (string.Equals(name, _indexFileNames[i], StringComparison.OrdinalIgnoreCase))
                    {
                        possibleIndexPages[i] = filename;
                        matchesIndexFileName = true;
                        break;
                    }
                }

                if (matchesIndexFileName)
                {
                    continue;
                }

                pages.Add(filename);
            }

            indexPageFileName = null;
            if (possibleIndexPages.Count > 0)
            {
                var bestIndex = possibleIndexPages.Keys.Min();
                indexPageFileName = possibleIndexPages[bestIndex];

                possibleIndexPages.Remove(bestIndex);

                foreach (var filename in possibleIndexPages.Values)
                {
                    pages.Add(filename);
                }
            }
            return pages;
        }

        private PageAsset BranchPage(string directory, HashSet<string> consumedFiles)
        {
            if (!ScanDirectory(
                directory,
                consumedFiles,
                out var metadata,
                out var indexPageFileName,
                out var pages))
            {
                return null;
            }

            var id = GetRelativePath(directory);
            ResourceId.Normalize(ref id);

            if (indexPageFileName == null ||
                !_contentHashProvider.TryGetContentHash(indexPageFileName, out var contentHash))
            {
                contentHash = "";
            }

            if (indexPageFileName == null)
            {
                if (pages.Count > 0)
                {
                    _report.Error($"Directory \"{id}\" has no index page and will be skipped");
                }

                return null;
            }


            var ctx = new ReadPageContext(_report.ForFile(indexPageFileName));
            var (content, pageMetadata) = _format.Read(ctx, indexPageFileName);
            metadata = metadata.MergeWith(pageMetadata);

            if (pages.Count == 0)
            {
                return new LeafPageAsset(
                    id: id,
                    relativePath: id,
                    absolutePath: indexPageFileName,
                    contentHash: contentHash,
                    content: content,
                    metadata: metadata
                );
            }

            return new BranchPageAsset(
                id: id,
                relativePath: id,
                absolutePath: indexPageFileName,
                contentHash: contentHash,
                content: content,
                metadata: metadata,
                subpages: pages.ToArray()
            );
        }

        private LeafPageAsset LeafPage(string path, HashSet<string> consumedFiles)
        {
            var id = GetRelativePath(path);
            id = Path.Combine(Path.GetDirectoryName(id), Path.GetFileNameWithoutExtension(id));
            ResourceId.Normalize(ref id);

            if (!_contentHashProvider.TryGetContentHash(path, out var contentHash))
            {
                contentHash = "";
            }

            var metadata = GetMetadata(path, consumedFiles, false);
            if (string.IsNullOrEmpty(metadata.Title))
            {
                metadata = metadata.WithTitle(Path.GetFileNameWithoutExtension(path));
            }

            var ctx = new ReadPageContext(_report.ForFile(path));
            var (content, pageMetadata) = _format.Read(ctx, path);
            metadata = metadata.MergeWith(pageMetadata);

            return new LeafPageAsset(
                id: id,
                relativePath: id,
                absolutePath: path,
                contentHash: contentHash,
                content: content,
                metadata: metadata
            );
        }

        private AttachmentAsset Attachment(string path)
        {
            var id = GetRelativePath(path);
            ResourceId.Normalize(ref id);

            if (!_contentTypeProvider.TryGetContentType(path, out var contentType))
            {
                contentType = AttachmentAsset.DEFAULT_MIME_TYPE;
            }

            if (!_contentHashProvider.TryGetContentHash(path, out var contentHash))
            {
                contentHash = "";
            }

            return new AttachmentAsset(
                id: id,
                relativePath: id,
                absolutePath: path,
                contentHash: contentHash,
                contentType: contentType
            );
        }

        private string GetRelativePath(string path)
        {
            try
            {
                var normalizedRootPath = Path.GetFullPath(RootDirectory);
                var normalizedPath = Path.GetFullPath(path);

                if (!normalizedPath.StartsWith(normalizedRootPath))
                {
                    throw new Exception($"\"{normalizedRootPath}\" is not a valid root for \"{normalizedPath}\"");
                }

                var relativePath = normalizedPath.Substring(normalizedRootPath.Length);
                relativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/');
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, '/');
                while (relativePath.Length > 0 && relativePath[0] == '/')
                {
                    relativePath = relativePath.Substring(1);
                }

                return relativePath;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"GetRelativePath(\"{RootDirectory}\", \"{path}\") failed", e);
            }
        }

        private PageMetadata GetMetadata(
            string filename,
            HashSet<string> consumedFiles,
            bool isIndexFile)
        {
            var metadata = _contentMetadataProvider.GetMetadata(
                sourceTreeRoot: _root,
                filename: filename,
                report: _report,
                consumedFiles: consumedFiles,
                isIndexFile: isIndexFile
            );

            if (string.IsNullOrEmpty(metadata.Title))
            {
                _report.ForFile(filename).Warning("No page title found");
            }

            return metadata;
        }

        private AttachmentAsset[] CollectAttachments(HashSet<string> consumedFiles)
        {
            var files = new List<AttachmentAsset>();
            CollectAttachmentsRec(RootDirectory);
            return files.ToArray();

            void CollectAttachmentsRec(string directory)
            {
                var ignoreRules = GetIgnoreRulesForDirectory(directory);
                foreach (var subdirectory in Directory.EnumerateDirectories(directory))
                {
                    CollectAttachmentsRec(subdirectory);
                }

                foreach (var filename in Directory.EnumerateFiles(directory))
                {
                    if (consumedFiles.Contains(filename))
                    {
                        continue;
                    }

                    if (ignoreRules.ShouldIgnore(filename))
                    {
                        continue;
                    }

                    consumedFiles.Add(filename);
                    files.Add(Attachment(filename));
                }
            }
        }

        private IIgnoreRule GetIgnoreRulesForDirectory(string path)
        {
            var ignoreRules = EnumerateIgnoreRulesForDirectory(path).ToArray();
            return new IgnoreRuleList(ignoreRules);
        }

        private IEnumerable<IIgnoreRule> EnumerateIgnoreRulesForDirectory(string path)
        {
            if (_ignorePatterns?.Length > 0)
            {
                yield return new SingleIgnoreRule(RootDirectory, _ignorePatterns);
            }

            foreach (var directory in WalkDirectoriesUp(path))
            {
                var mdIgnoreFileName = Path.Combine(directory, MdIgnoreFileRule.FileName);
                if (File.Exists(mdIgnoreFileName))
                {
                    yield return new MdIgnoreFileRule(directory, mdIgnoreFileName);
                }
            }
        }

        private IEnumerable<string> WalkDirectoriesUp(string path)
        {
            var directory = path;
            directory = Path.GetFullPath(directory);

            while (directory != null && directory != RootDirectory)
            {
                yield return directory;
                directory = Path.GetDirectoryName(directory);
            }
            yield return RootDirectory;
        }

        #endregion
    }
}