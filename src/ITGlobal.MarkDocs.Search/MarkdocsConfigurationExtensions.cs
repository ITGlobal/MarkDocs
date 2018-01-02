using JetBrains.Annotations;
using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Search
{

    /// <summary>
    ///     Extension methods for <see cref="MarkDocsExtensionConfigurationBuilder"/> to configure search extension
    /// </summary>
    [PublicAPI]
    public static class MarkdocsConfigurationExtensions
    {
        /// <summary>
        ///     Adds search support into MarkDocs
        /// </summary>
        /// <param name="configuration">
        ///     Configuration builder
        /// </param>
        /// <param name="indexDirectory">
        ///     Path to lucene index directory
        /// </param>
        [PublicAPI]
        public static void AddSearch(
            [NotNull] this MarkDocsExtensionConfigurationBuilder configuration,
            [NotNull] string indexDirectory)
        {
            AddSearch(configuration, new SearchOptions { IndexDirectory = indexDirectory });
        }

        /// <summary>
        ///     Adds search support into MarkDocs
        /// </summary>
        /// <param name="configuration">
        ///     Configuration builder
        /// </param>
        /// <param name="options">
        ///     Options for search extension
        /// </param>
        [PublicAPI]
        public static void AddSearch(
            [NotNull] this MarkDocsExtensionConfigurationBuilder configuration,
            [NotNull] SearchOptions options)
        {
            configuration.Add(ctx =>
            {
                var searchEngine = new LuceneSearchEngine(ctx.LoggerFactory, options);
                return new SearchExtensionFactory(searchEngine);
            });
        }
    }
}
