using System;
using ITGlobal.MarkDocs.Format;
using Microsoft.AspNetCore.StaticFiles;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class AssetTreeReader : IAssetTreeReader
    {
        private readonly IFormat _format;
        private readonly IContentHashProvider _contentHashProvider;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly IContentMetadataProvider _contentMetadataProvider;
        private readonly IResourceUrlResolver _resourceUrlResolver;
        private readonly IMarkDocsLog _log;
        private readonly string[] _includeFiles;
        private readonly string[] _indexFileNames;

        public AssetTreeReader(
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
            _indexFileNames = format.IndexFileNames;
        }

        public AssetTree ReadAssetTree(
            ISourceTreeProvider sourceTreeProvider, 
            ISourceTreeRoot sourceTreeRoot, 
            ICompilationReportBuilder report)
        {
            var worker = new AssetTreeReaderWorker(
                format: _format,
                contentHashProvider: _contentHashProvider,
                contentTypeProvider: _contentTypeProvider,
                contentMetadataProvider: _contentMetadataProvider,
                resourceUrlResolver: _resourceUrlResolver,
                includeFiles: _includeFiles,
                indexFileNames: _indexFileNames,
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