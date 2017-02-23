using System.Text;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Defines methods and filters to discover, parse and render pages
    /// </summary>
    [PublicAPI]
    public interface IFormat
    {
        /// <summary>
        ///     Get a list of page file filters
        /// </summary>
        [NotNull]
        string[] FileFilters { get; }

        /// <summary>
        ///     Get a list of index page file filters
        /// </summary>
        [NotNull]
        string[] IndexFileNames { get; }

        /// <summary>
        ///     Encoding for source files
        /// </summary>
        [NotNull]
        Encoding SourceEncoding { get; }

        /// <summary>
        ///     Reads a page file <paramref name="filename"/> and parses it's metadata (see <see cref="Metadata"/>)
        /// </summary>
        [NotNull]
        Metadata ParseProperties([NotNull] string filename);

        /// <summary>
        ///     Renders content of <paramref name="markup"/> into HTML
        /// </summary>
        [NotNull]
        string Render([NotNull] IRenderContext ctx, [NotNull] string markup);
    }
}