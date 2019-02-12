using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Provides asset metadata from various sources
    /// </summary>
    [PublicAPI]
    public interface IContentMetadataProvider
    {
        /// <summary>
        ///     Gets a metadata for a specified page
        /// </summary>
        /// <param name="sourceTreeRoot">
        ///     Content root directory
        /// </param>
        /// <param name="filename">
        ///     Page file name
        /// </param>
        /// <param name="report">
        ///     Compilation report builder
        /// </param>
        /// <param name="isIndexFile">
        ///     true is <paramref name="filename"/> is an index page file.
        /// </param>
        /// <returns>
        ///     Page metadata if available, null otherwise
        /// </returns>
        [NotNull]
        PageMetadata GetMetadata(
            [NotNull] ISourceTreeRoot sourceTreeRoot,
            [NotNull] string filename,
            [NotNull] ICompilationReportBuilder report,
            bool isIndexFile
        );
    }
}