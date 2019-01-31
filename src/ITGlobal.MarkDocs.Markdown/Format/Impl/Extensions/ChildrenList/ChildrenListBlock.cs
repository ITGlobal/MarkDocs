using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.ChildrenList
{
    internal sealed class ChildrenListBlock : Block
    {
        public ChildrenListBlock(BlockParser parser, MarkdownDocument document)
            : base(parser)
        {
            Document = document;
        }

        public MarkdownDocument Document { get; }
    }
}