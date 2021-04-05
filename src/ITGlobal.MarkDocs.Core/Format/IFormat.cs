using System.Collections.Generic;
using System.Text;
using ITGlobal.MarkDocs.Source;
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
        ///     Supported extensions
        /// </summary>
        [NotNull]
        ISet<string> Extensions { get; }

        /// <summary>
        ///     Parses content of file <paramref name="filename"/>
        /// </summary>
        (IPageContent, PageMetadata) Read([NotNull] IPageReadContext ctx, [NotNull] string filename);

    }
}