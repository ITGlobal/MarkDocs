using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     Page tree node (base class)
    /// </summary>
    [DebuggerDisplay("{Id}")]
    internal abstract class PageTreeNode : IPageTreeNode
    {
        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        protected PageTreeNode(string id, Metadata metadata)
        {
            ResourceId.Normalize(ref id);

            Id = id;
            Metadata = metadata;
        }
        
        #endregion

        #region IPageTreeNode

        /// <summary>
        ///     Node ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        public IDocumentation Documentation { get; private set; }

        /// <summary>
        ///     Node title
        /// </summary>
        public string Title => Metadata.Title;

        /// <summary>
        ///     true if this node refers to a page
        /// </summary>
        public abstract bool IsHyperlink { get; }

        /// <summary>
        ///     A reference to a parent node. Null for root nodes
        /// </summary>
        public IPageTreeNode Parent { get; set; }

        /// <summary>
        ///     Child nodes. Null for leaf nodes.
        /// </summary>
        public abstract IReadOnlyList<IPageTreeNode> Nodes { get; }

        #endregion

        #region properties

        /// <summary>
        ///     File name
        /// </summary>
        public abstract string FileName { get; }

        /// <summary>
        ///     File name (relative to root directory)
        /// </summary>
        public abstract string RelativeFileName { get; }

        /// <summary>
        ///     Page metadata
        /// </summary>
        public Metadata Metadata { get; }

        /// <summary>
        ///     Node order
        /// </summary>
        public int Order => Metadata.Order;

        /// <summary>
        ///     Node kind order (directories come first)
        /// </summary>
        public abstract int NodeKindOrder { get; }

        /// <summary>
        ///     Resource type
        /// </summary>
        public ResourceType Type => ResourceType.Page;

        #endregion

        #region methods

        /// <summary>
        ///     Executes a specified action for this node and each descendant nodes
        /// </summary>
        public void ScanNodes(Action<PageTreeNode, int> action, int level = 0)
        {
            action(this, level);

            if (Nodes == null)
            {
                return;
            }

            foreach (var node in Nodes.OfType<PageTreeNode>())
            {
                node.ScanNodes(action, level + 1);
            }
        }

        /// <summary>
        ///     Links a node and all its chilren to documentation object
        /// </summary>
        public void LinkToDocumentation(IDocumentation documentation)
        {
            ScanNodes((node, _) => node.Documentation = documentation);
        }

        #endregion
    }
}