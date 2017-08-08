using JetBrains.Annotations;
using ITGlobal.MarkDocs.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

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
            configuration.Add(services =>
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var engine = new LuceneSearchEngine(loggerFactory, options.IndexDirectory);
                return new SearchExtensionFactory(engine);
            });
        }
    }
}
