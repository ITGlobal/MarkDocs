using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Admonition
{
    /// <summary>
    ///     An admonition markup block
    /// </summary>
    public sealed class AdmonitionBlock : LeafBlock
    {
        internal AdmonitionBlock(BlockParser parser)
            : base(parser)
        {
            ProcessInlines = true;
        }

        /// <summary>
        ///     Admonition type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Admonition title
        /// </summary>
        public string Title { get; set; }
    }
}