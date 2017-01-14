using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Directory page tree node
    /// </summary>
    internal class DirectoryPageTreeNode : PageTreeNode
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public DirectoryPageTreeNode(string id, Metadata metadata, string filename, string relativeFileName, IReadOnlyList<IPageTreeNode> nodes)
            : base(id, metadata)
        {
            FileName = filename;
            RelativeFileName = relativeFileName;
            Nodes = nodes
                .OfType<PageTreeNode>()
                .OrderBy(_ => _.Order != 0 ? _.Order : int.MaxValue)
                .ThenBy(_ => _.Title, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(_ => _.NodeKindOrder)
                .ToArray();

            foreach (var node in nodes.OfType<PageTreeNode>())
            {
                node.Parent = this;
            }
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
        public override bool IsHyperlink => FileName != null;

        /// <summary>
        ///     Child nodes. Null for leaf nodes.
        /// </summary>
        public override IReadOnlyList<IPageTreeNode> Nodes { get; }

        /// <summary>
        ///     Node kind order (directories come first)
        /// </summary>
        public override int NodeKindOrder => 1;
    }
}