using System.IO;
using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A documentation page
    /// </summary>
    [PublicAPI]
    public interface IPage
    {
        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        [PublicAPI, NotNull]
        IDocumentation Documentation { get; }

        /// <summary>
        ///     Page ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }

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