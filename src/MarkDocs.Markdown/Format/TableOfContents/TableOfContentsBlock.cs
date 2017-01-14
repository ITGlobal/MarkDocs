using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.TableOfContents
{
    internal sealed class TableOfContentsBlock : Block
    {
        public TableOfContentsBlock(BlockParser parser, MarkdownDocument document)
            : base(parser)
        {
            Document = document;
        }

        public MarkdownDocument Document { get; }
    }
}
