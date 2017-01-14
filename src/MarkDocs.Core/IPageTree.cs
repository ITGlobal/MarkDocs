using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Page tree
    /// </summary>
    [PublicAPI]
    public interface IPageTree
    {
        /// <summary>
        ///     List of root pages
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IPageTreeNode> Nodes { get; }

        /// <summary>
        ///     Flat list of pages
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IPageTreeNode> Pages { get; }
    }
}