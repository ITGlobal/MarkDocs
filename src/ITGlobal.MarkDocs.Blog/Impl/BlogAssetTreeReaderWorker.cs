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
            ICompilationReportBuilder report,
            IMarkDocsLog log)
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
            Log = log;
        }

        #endregion

        #region metadata

        public ISourceTreeRoot Root { get; }
        public IMarkDocsLog Log { get; }
        public string RootDirectory => Root.RootDirectory;
        public IFormat Format { get; }
        public ICompilationReportBuilder Report { get; }
        public IResourceUrlResolver ResourceUrlResolver { get; }

        #endregion

        #region public methods

        public AssetTree ReadAssetTree()
        {
            Log.Debug($"Scanning directory \"{RootDirectory}\"...");
            var filepathes = ScanDirectoryFiles(RootDirectory).ToHashSet(StringComparer.OrdinalIgnoreCase);

            Log.Debug($"Reading assets from \"{RootDirectory}\"...");
            foreach (var shallowPage in  LoadShallowPages())
            {
                _pages[shallowPage.Id] = shallowPage;
            }

            var pages = LoadFullPages(_pages.Values).ToArray();
            foreach (var page in pages)
            {
                filepathes.Remove(page.AbsolutePath);
                
            }

            // Make a list of referenced files
            foreach (var (_, asset) in _files)
            {
                if(asset is PhysicalFileAsset fileAsset)
                {
                    filepathes.Remove(fileAsset.AbsolutePath);
                }
            }

            foreach (var path in filepathes)
            {
                var id = GetRelativePath(path);
                ResourceId.Normalize(ref id);
                ResolveFileAsset(id);
            }
            
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

            tree.Validate(new PageValidateContext(this));

            return tree;

            IEnumerable<ShallowPageAsset> LoadShallowPages()
            {
                foreach (var file in filepathes)
                {
                    var ext = Path.GetExtension(file);
                    if (Format.Extensions.Contains(ext))
                    {
                        var shallowPage = LeafPage(file);
                        if (shallowPage != null)
                        {
                            yield return shallowPage;
                        }
                    }
                }
            }

            IEnumerable<PageAsset> LoadFullPages(IEnumerable<ShallowPageAsset> shallowPages)
            {
                foreach (var shallowPage in shallowPages)
                {
                    var page = shallowPage.ReadAsset(this);
                    if (page != null)
                    {
                        yield return page;
                    }
                }
            }
        }

        public PageMetadata GetMetadata(string filename, bool isIndexFile)
        {
            var metadata = _contentMetadataProvider.GetMetadata(
                sourceTreeRoot: Root,
                filename: filename,
                report: Report,
                isIndexFile: isIndexFile
            );

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

            var filename = Path.GetFileName(id);
            var isIndexFile = false;
            foreach (var indexFileName in Format.IndexFileNames)
            {
                if (string.Equals(indexFileName, filename, StringComparison.OrdinalIgnoreCase))
                {
                    isIndexFile = true;
                    break;
                }
            }

            id = Path.GetDirectoryName(id);
            if (!isIndexFile)
            {
                id = Path.Combine(id, Path.GetFileNameWithoutExtension(path));
            }
            
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
                        
                        //var files = from filter in _includeFiles
                        //    from filename in Directory.EnumerateFiles(dayDir, filter)
                        //    select filename;
                        var files = from filename in Directory.EnumerateFiles(dayDir, "*")
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