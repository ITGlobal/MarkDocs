using System.Collections.Generic;
using JetBrains.Annotations;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A TOC tree node
    /// </summary>
    [PublicAPI]
    public interface ITocTreeNode
    {
        /// <summary>
        ///     Title text
        /// </summary>
        [NotNull]
        string Title { get; }

        /// <summary>
        ///     Heading block
        /// </summary>
        [CanBeNull]
        HeadingBlock Heading { get; }
        
        /// <summary>
        ///     Depth level
        /// </summary>
        int Level { get; }

        /// <summary>
        ///     List of child nodes
        /// </summary>
        [NotNull]
        IReadOnlyList<ITocTreeNode> Children { get; }
    }
}