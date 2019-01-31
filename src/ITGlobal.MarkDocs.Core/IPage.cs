using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using System.Collections.Generic;
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
        [PublicAPI, NotNull]
        string Title { get; }

        /// <summary>
        ///     Page description
        /// </summary>
        [PublicAPI, CanBeNull]
        string Description { get; }

        /// <summary>
        ///     Page metadata
        /// </summary>
        [PublicAPI, NotNull]
        PageMetadata Metadata { get; }

        /// <summary>
        ///     Page preview if defined, null otherwise
        /// </summary>
        [CanBeNull]
        IPagePreview Preview { get; }

        /// <summary>
        ///     Page anchors (with names)
        /// </summary>
        IReadOnlyDictionary<string, string> Anchors { get; }

        /// <summary>
        ///     Nested pages
        /// </summary>
        [PublicAPI, NotNull]
        IPage[] NestedPages { get; }

        /// <summary>
        ///     A reference to a parent page. Null for root nodes
        /// </summary>
        [PublicAPI, CanBeNull]
        IPage Parent { get; }

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        [NotNull]
        Stream OpenRead();
    }
}