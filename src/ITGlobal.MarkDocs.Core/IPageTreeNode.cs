using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Page tree node
    /// </summary>
    [PublicAPI]
    public interface IPageTreeNode
    {
        /// <summary>
        ///     Node ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }

        /// <summary>
        ///     Node title
        /// </summary>
        [PublicAPI, NotNull]
        string Title { get; }
        
        /// <summary>
        ///     true if this node refers to a page
        /// </summary>
        [PublicAPI]
        bool IsHyperlink { get; }

        /// <summary>
        ///     A reference to a parent node. Null for root nodes
        /// </summary>
        [PublicAPI, CanBeNull]
        IPageTreeNode Parent { get; }

        /// <summary>
        ///     Child nodes. Null for leaf nodes.
        /// </summary>
        [PublicAPI, CanBeNull]
        IReadOnlyList<IPageTreeNode> Nodes { get; }
    }
}