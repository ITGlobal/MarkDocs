using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Impl;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class AssetTreeReaderWorker : IShallowPageAssetReader
    {
        #region fields

        private readonly IContentHashProvider _contentHashProvider;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IContentMetadataProvider _contentMetadataProvider;
        private readonly string[] _includeFiles;
        private readonly string[] _indexFileNames;
        private readonly string[] _ignorePatterns;

        private readonly Dictionary<string, ShallowPageAsset> _pages
            = new Dictionary<string, ShallowPageAsset>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, FileAsset> _files
            = new Dictionary<string, FileAsset>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region .ctor

        public AssetTreeReaderWorker(
            IFormat format,
            IContentHashProvider contentHashProvider,
            IContentTypeProvider contentTypeProvider,
            IContentMetadataProvider contentMetadataProvider,
            IResourceUrlResolver resourceUrlResolver,
            string[] includeFiles,
            string[] indexFileNames,
            string[] ignorePatterns,
            ISourceTreeRoot root,
            ICompilationReportBuilder report)
        {
            Format = format;

            _contentHashProvider = contentHashProvider;
            _contentTypeProvider = contentTypeProvider;
            _contentMetadataProvider = contentMetadataProvider;
            ResourceUrlResolver = resourceUrlResolver;

            _includeFiles = includeFiles;
            _indexFileNames = indexFileNames;
            _ignorePatterns = ignorePatterns;

            Root = root;
            Report = report;
        }

        #endregion

        #region metadata

        public ISourceTreeRoot Root { get; }
        public string RootDirectory => Root.RootDirectory;
        public IFormat Format { get; }
        public ICompilationReportBuilder Report { get; }
        public IResourceUrlResolver ResourceUrlResolver { get; }

        #endregion

        #region public methods

        public AssetTree ReadAssetTree()
        {
            // Scan directory tree and collect "shallow" assets
            var rootAsset = BranchPage(RootDirectory);
            rootAsset.ForEach(p => _pages[p.Id] = p);

            // Read each asset and convert "shallow" assets into "full" ones
            var rootPage = rootAsset.ReadAsset(this);

            // Make a list of referenced files
            var files = _files.Values.Where(_ => _ != null).OrderBy(_ => _.Id).ToArray();

            // Assemble an asset tree
            var tree = new AssetTree(
                Root.SourceTree.Id,
                RootDirectory,
                Root.SourceInfo,
                rootPage,
                files
            );
            return tree;
        }

        public PageMetadata GetMetadata(string filename, bool isIndexFile)
        {
            var metadata = _contentMetadataProvider.GetMetadata(
                sourceTreeRoot: Root,
                filename: filename,
                report: Report,
                isIndexFile: isIndexFile
            );

            if (string.IsNullOrEmpty(metadata.Title))
            {
                Report.Warning(filename, "No page title found");
            }

            return metadata;
        }

        public ShallowPageAsset ResolvePageAsset(string path)
        {
            if (!_pages.TryGetValue(path, out var page))
            {
                return null;
            }

            return page;
        }

        public FileAsset ResolveFileAsset(string path)
        {
            if (_files.TryGetValue(path, out var file))
            {
                return file;
            }

            file = PhysicalFile(GetAbsolutePath(path));
            _files[path] = file;

            return file;
        }

        public void RegisterAsset(GeneratedFileAsset asset)
        {
            _files[asset.Id] = asset;
        }

        #endregion

        #region asset factories

        private ShallowPageAsset BranchPage(string directory)
        {
            if (!ScanDirectory(
                directory,
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
                    Report.Error(directory, $"Directory \"{id}\" has no index page and will be skipped");
                }

                return null;
            }

            if (pages.Count == 0)
            {
                return new LeafShallowPageAsset(
                    id: id,
                    relativePath: id,
                    absolutePath: indexPageFileName,
                    contentHash: contentHash
                );
            }

            return new BranchShallowPageAsset(
                id: id,
                relativePath: id,
                absolutePath: indexPageFileName,
                contentHash: contentHash,
                subpages: pages.ToArray()
            );
        }

        private LeafShallowPageAsset LeafPage(string path)
        {
            var id = GetRelativePath(path);
            id = Path.Combine(Path.GetDirectoryName(id), Path.GetFileNameWithoutExtension(id));
            ResourceId.Normalize(ref id);

            if (!_contentHashProvider.TryGetContentHash(path, out var contentHash))
            {
                contentHash = "";
            }

            return new LeafShallowPageAsset(
                id: id,
                relativePath: id,
                absolutePath: path,
                contentHash: contentHash
            );
        }

        private PhysicalFileAsset PhysicalFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var ignoreRules = GetIgnoreRulesForDirectory(Path.GetDirectoryName(path));
            if (ignoreRules.ShouldIgnore(path))
            {
                return null;
            }

            var id = GetRelativePath(path);
            ResourceId.Normalize(ref id);

            if (!_contentTypeProvider.TryGetContentType(path, out var contentType))
            {
                contentType = Asset.DEFAULT_MIME_TYPE;
            }

            if (!_contentHashProvider.TryGetContentHash(path, out var contentHash))
            {
                contentHash = "";
            }

            return new PhysicalFileAsset(
                id: id,
                relativePath: id,
                absolutePath: path,
                contentHash: contentHash,
                contentType: contentType
            );
        }

        #endregion

        #region directory traverse

        private bool ScanDirectory(
            string directory,
            out string indexPageFileName,
            out List<ShallowPageAsset> pages)
        {
            var ignoreRules = GetIgnoreRulesForDirectory(directory);

            pages = new List<ShallowPageAsset>();
            foreach (var subDirectory in Directory.EnumerateDirectories(directory))
            {
                if (ignoreRules.ShouldIgnore(subDirectory))
                {
                    continue;
                }

                var page = BranchPage(subDirectory);
                if (page != null)
                {
                    pages.Add(page);
                }
            }

            var pageFiles = ScanDirectoryFiles(directory, out indexPageFileName);
            foreach (var filename in pageFiles)
            {
                pages.Add(LeafPage(filename));
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

        private string GetAbsolutePath(string path) => PathHelper.GetAbsolutePath(RootDirectory, path);

        private string GetRelativePath(string path) => PathHelper.GetRelativePath(RootDirectory, path);

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
