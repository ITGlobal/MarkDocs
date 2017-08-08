using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Markdown.Format
{
    public sealed class AdmonitionBlock : LeafBlock
    {
        internal AdmonitionBlock(BlockParser parser)
            : base(parser)
        {
            ProcessInlines = true;
        }

        public string Type { get; set; }
        public string Title { get; set; }
    }
}