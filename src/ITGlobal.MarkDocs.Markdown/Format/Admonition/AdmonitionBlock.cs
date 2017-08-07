using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Markdown.Format.Admonition
{
    internal sealed class AdmonitionBlock : LeafBlock
    {
        public AdmonitionBlock(BlockParser parser)
            : base(parser)
        {
            ProcessInlines = true;
        }

        public string Type { get; set; }
        public string Title { get; set; }
    }
}