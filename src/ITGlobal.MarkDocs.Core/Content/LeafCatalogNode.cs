using System.Collections.Generic;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Leaf page tree node
    /// </summary>
    internal class LeafPageTreeNode : PageTreeNode
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public LeafPageTreeNode(string id, Metadata metadata, string filename, string relativeFileName) 
            : base(id, metadata)
        {
            FileName = filename;
            RelativeFileName = relativeFileName;
        }

        /// <summary>
        ///     File name
        /// </summary>
        public override string FileName { get; }

        /// <summary>
        ///     File name (relative to root directory)
        /// </summary>
        public override string RelativeFileName { get; }

        /// <summary>
        ///     true if this node refers to a page
        /// </summary>
        public override bool IsHyperlink => true;

        /// <summary>
        ///     true if this node refers to an index page
        /// </summary>
        public override bool IsIndexPage => false;

        /// <summary>
        ///     Child nodes. Null for leaf nodes.
        /// </summary>
        public override IReadOnlyList<IPageTreeNode> Nodes => null;

        /// <summary>
        ///     Node kind order (directories come first)
        /// </summary>
        public override int NodeKindOrder => 2;
    }
}