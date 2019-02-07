using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Metadata
{
    internal interface IMetadataExtractor
    {
        [NotNull]
        PageMetadata Extract([NotNull] IPageReadContext ctx, [NotNull] MarkdownDocument document);
    }
}