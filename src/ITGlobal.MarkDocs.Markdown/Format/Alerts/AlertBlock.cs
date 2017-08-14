using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Markdown.Format
{
    /// <summary>
    ///     An alert markup block
    /// </summary>
    public sealed class AlertBlock : ContainerBlock
    {
        internal AlertBlock(BlockParser parser)
            : base(parser)
        { }

        /// <summary>
        ///     Alert type
        /// </summary>
        public AlertBlockType Type { get; set; }

        internal int Level { get; set; }
    }
}
