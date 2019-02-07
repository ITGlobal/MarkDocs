using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using System.IO;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A documentation page
    /// </summary>
    [PublicAPI]
    public interface IPage : IResource
    {
        /// <summary>
        ///     Page title
        /// </summary>
        [NotNull]
        string Title { get; }

        /// <summary>
        ///     Page description
        /// </summary>
        [CanBeNull]
        string Description { get; }

        /// <summary>
        ///     Page metadata
        /// </summary>
        [NotNull]
        PageMetadata Metadata { get; }

        /// <summary>
        ///     Page preview if defined, null otherwise
        /// </summary>
        [CanBeNull]
        IPagePreview Preview { get; }

        /// <summary>
        ///     Page anchors (with names)
        /// </summary>
        [NotNull]
        PageAnchors Anchors { get; }

        /// <summary>
        ///     Nested pages
        /// </summary>
        [NotNull]
        IPage[] NestedPages { get; }

        /// <summary>
        ///     A reference to a parent page. Null for root nodes
        /// </summary>
        [CanBeNull]
        IPage Parent { get; }

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        [NotNull]
        Stream OpenRead();
    }
}