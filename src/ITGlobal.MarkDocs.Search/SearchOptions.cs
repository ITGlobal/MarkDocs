using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     Options for search extension
    /// </summary>
    [PublicAPI]
    public sealed class SearchOptions
    {
        /// <summary>
        ///     Path to lucene index directory
        /// </summary>
        [NotNull]
        public string IndexDirectory { get; set; }

        /// <summary>
        ///     True to enable verbose logging
        /// </summary>
        public bool VerboseLogging { get; set; }
    }
}
