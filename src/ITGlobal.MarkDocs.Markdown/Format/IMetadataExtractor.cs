using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format
{
    internal interface IMetadataExtractor
    {
        void TryExtract(MarkdownDocument document, Metadata metadata);
    }
}