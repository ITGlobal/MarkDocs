using System.Linq;
using ITGlobal.MarkDocs.Format.Impl.Extensions.CustomHeading;
using ITGlobal.MarkDocs.Source;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Metadata
{
    internal sealed class HeadingMetadataExtractor : IMetadataExtractor
    {
        public PageMetadata Extract(IReadPageContext ctx, MarkdownDocument document)
        {
            HeadingBlock header = null;
            string title = null;

            for (var level = 1; level <= 2; level++)
            {
                var success = TryParseTitleFromHeader(document, level, out title, out header);
                if (success)
                {
                    break;
                }
            }

            var properties = PageMetadata.Empty;
            if (header != null)
            {
                CustomHeadingRenderer.DontRender(header);
                properties = properties.WithTitle(title);
            }

            return properties;
        }

        private static bool TryParseTitleFromHeader(
            MarkdownDocument document,
            int level,
            out string title,
            out HeadingBlock header)
        {
            header = document.Descendants<HeadingBlock>().FirstOrDefault(_ => _.Level == level);
            title = header?.Inline.GetText();

            return !string.IsNullOrEmpty(title);
        }
    }
}