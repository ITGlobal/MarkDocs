﻿namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class DefaultContentMetadataProvider : IContentMetadataProvider
    {
        private readonly IContentMetadataProvider[] _providers;

        public DefaultContentMetadataProvider()
        {
            _providers = new IContentMetadataProvider[]
            {
                new TocContentMetadataProvider(),
                new SourceTreeContentMetadataProvider(),
            };
        }

        public PageMetadata GetMetadata(
            ISourceTreeRoot rootDirectory,
            string filename,
            ICompilationReportBuilder report,
            bool isIndexFile)
        {
            var metadata = PageMetadata.Empty;
            foreach (var provider in _providers)
            {
                var m = provider.GetMetadata(rootDirectory, filename, report, isIndexFile);
                metadata = metadata.MergeWith(m);
            }

            return metadata;
        }
    }
}