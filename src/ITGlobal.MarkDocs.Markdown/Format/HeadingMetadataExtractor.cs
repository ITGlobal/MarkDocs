using System.Linq;
using ITGlobal.MarkDocs.Format.CustomHeading;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format
{
    internal sealed class HeadingMetadataExtractor : IMetadataExtractor
    {
        public void TryExtract(MarkdownDocument document, Metadata metadata)
        {
            HeadingBlock header = null;
            string title = null;
            int order = 0;

            for (var level = 1; level <= 2; level++)
            {
                var success = TryParseTitleFromHeader(document, level, out title, out header);
                if (success)
                {
                    break;
                }
            }

            if (header == null)
            {
                return;
            }

            CustomHeadingRenderer.DontRender(header);

            metadata.Title = title;
            metadata.Order = order;
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