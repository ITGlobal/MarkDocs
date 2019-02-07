using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Impl;
using ITGlobal.MarkDocs.Source;
using ITGlobal.MarkDocs.Source.Impl;
using Microsoft.AspNetCore.StaticFiles;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class BlogAssetTreeReaderWorker : IShallowPageAssetReader
    {
        #region fields

        private readonly IContentHashProvider _contentHashProvider;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IContentMetadataProvider _contentMetadataProvider;
        private readonly string[] _includeFiles;
        private readonly string[] _ignorePatterns;

        private readonly Dictionary<string, ShallowPageAsset> _pages
            = new Dictionary<string, ShallowPageAsset>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, FileAsset> _files
            = new Dictionary<string, FileAsset>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region .ctor

        public BlogAssetTreeReaderWorker(
            IFormat format,
            IContentHashProvider contentHashProvider,
            IContentTypeProvider contentTypeProvider,
            IContentMetadataProvider contentMetadataProvider,
            IResourceUrlResolver resourceUrlResolver,
            string[] includeFiles,
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
            var shallowAssets = ScanDirectoryFiles(RootDirectory).Select(LeafPage).ToList();

            var pages = shallowAssets.Select(asset => asset.ReadAsset(this)).Where(_ => _ != null).ToArray();

            // Make a list of referenced files
            var files = _files.Values.Where(_ => _ != null).OrderBy(_ => _.Id).ToArray();

            // Assemble an asset tree
            var tree = new AssetTree(
                Root.SourceTree.Id,
                RootDirectory,
                Root.SourceInfo,
                new BranchPageAsset(
                    "/",
                    "/",
                    RootDirectory,
                    "0000000000000000000000000000000000000000",
                    new BlogRootPageContent(),
                    PageMetadata.Empty,
                    pages
                ),
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

        private IEnumerable<string> ScanDirectoryFiles(string directory)
        {
            var ignoreRules = GetIgnoreRulesForDirectory(directory);

            foreach (var yearDir in Directory.EnumerateDirectories(directory))
            {
                if (ignoreRules.ShouldIgnore(yearDir))
                {
                    continue;
                }

                if (!int.TryParse(Path.GetFileName(yearDir), out var year))
                {
                    continue;
                }

                foreach (var monthDir in Directory.EnumerateDirectories(yearDir))
                {
                    if (ignoreRules.ShouldIgnore(monthDir))
                    {
                        continue;
                    }

                    if (!int.TryParse(Path.GetFileName(monthDir), out var month))
                    {
                        continue;
                    }


                    foreach (var dayDir in Directory.EnumerateDirectories(monthDir))
                    {
                        if (ignoreRules.ShouldIgnore(dayDir))
                        {
                            continue;
                        }

                        if (!int.TryParse(Path.GetFileName(dayDir), out var day))
                        {
                            continue;
                        }

                        try
                        {
                            _ = new DateTime(year, month, day);
                        }
                        catch
                        {
                            continue;
                        }
                        
                        var files = from filter in _includeFiles
                            from filename in Directory.EnumerateFiles(dayDir, filter)
                            select filename;

                        foreach (var filename in files)
                        {
                            if (ignoreRules.ShouldIgnore(filename))
                            {
                                continue;
                            }

                            yield return filename;
                        }
                    }
                }
            }

          
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