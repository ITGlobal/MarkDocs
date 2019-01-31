using ITGlobal.MarkDocs.Source;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Metadata
{
    internal sealed class DefaultMetadataExtractor : IMetadataExtractor
    {
        private readonly IMetadataExtractor[] _extractors = 
        {
            new HeadingMetadataExtractor(),
            new YamlMetadataExtractor()
        };

        public PageMetadata Extract(IReadPageContext ctx, MarkdownDocument document)
        {
            var metadata = PageMetadata.Empty;
            foreach (var extractor in _extractors)
            {
                metadata = metadata.MergeWith(extractor.Extract(ctx, document));
            }

            return metadata;
        }
    }
}