using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Source;
using Microsoft.AspNetCore.StaticFiles;
using System;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class BlogAssetTreeReader : IAssetTreeReader
    {
        private readonly IFormat _format;
        private readonly IContentHashProvider _contentHashProvider;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IContentMetadataProvider _contentMetadataProvider;
        private readonly IResourceUrlResolver _resourceUrlResolver;
        private readonly IMarkDocsLog _log;
        private readonly string[] _includeFiles;

        public BlogAssetTreeReader(
            IFormat format,
            IContentHashProvider contentHashProvider,
            IContentTypeProvider contentTypeProvider,
            IContentMetadataProvider contentMetadataProvider,
            IResourceUrlResolver resourceUrlResolver,
            IMarkDocsLog log)
        {
            _format = format;
            _contentHashProvider = contentHashProvider;
            _contentTypeProvider = contentTypeProvider;
            _contentMetadataProvider = contentMetadataProvider;
            _resourceUrlResolver = resourceUrlResolver;
            _log = log;

            _includeFiles = format.FileFilters;
        }

        public AssetTree ReadAssetTree(
            ISourceTreeProvider sourceTreeProvider,
            ISourceTreeRoot sourceTreeRoot,
            ICompilationReportBuilder report)
        {
            var worker = new BlogAssetTreeReaderWorker(
                format: _format,
                contentHashProvider: _contentHashProvider,
                contentTypeProvider: _contentTypeProvider,
                contentMetadataProvider: _contentMetadataProvider,
                resourceUrlResolver: _resourceUrlResolver,
                includeFiles: _includeFiles,
                ignorePatterns: sourceTreeProvider.IgnorePatterns ?? Array.Empty<string>(),
                root: sourceTreeRoot,
                report: report,
                log: _log
            );

            var tree = worker.ReadAssetTree();
            return tree;
        }
    }
}
