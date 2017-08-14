using System.Collections.Generic;
using System.IO;
using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;

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
        ///     Page tree node that refers to this page
        /// </summary>
        [PublicAPI, NotNull]
        IPageTreeNode PageTreeNode { get; }

        /// <summary>
        ///     Page metadata
        /// </summary>
        [PublicAPI, NotNull]
        Metadata Metadata { get; }

        /// <summary>
        ///     Page anchors (with names)
        /// </summary>
        IReadOnlyDictionary<string, string> Anchors { get; }

        /// <summary>
        ///     Reads page source markup
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        [PublicAPI, NotNull]
        Stream ReadMarkup();

        /// <summary>
        ///     Reads page rendered HTML
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        [PublicAPI, NotNull]
        Stream ReadHtml();
    }
}