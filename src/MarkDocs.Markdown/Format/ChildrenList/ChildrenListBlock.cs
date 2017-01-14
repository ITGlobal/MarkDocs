using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.ChildrenList
{
    internal sealed class ChildrenListBlock : Block
    {
        public ChildrenListBlock(BlockParser parser, MarkdownDocument document, IPage page)
            : base(parser)
        {
            Document = document;
            Page = page;
        }

        public MarkdownDocument Document { get; }
        public IPage Page { get; }
    }
}