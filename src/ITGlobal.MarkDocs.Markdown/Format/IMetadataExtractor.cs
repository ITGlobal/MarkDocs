using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format
{
    internal interface IMetadataExtractor
    {
        void TryExtract(IParsePropertiesContext ctx, MarkdownDocument document, Metadata metadata);
    }
}