using System;
using ITGlobal.MarkDocs.Extensions;
using JetBrains.Annotations;

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
        /// <param name="searchEngineFactory">
        ///     Search engine factory
        /// </param>
        [PublicAPI]
        public static void AddSearch(
            [NotNull] this MarkDocsExtensionConfigurationBuilder configuration,
            [NotNull] Func<IServiceProvider, ISearchEngine> searchEngineFactory)
        {
            configuration.Add(sp =>
            {
                var searchEngine = searchEngineFactory(sp);
                var factory = new SearchExtensionFactory(searchEngine);
                return factory;
            });
        }

        /// <summary>
        ///     Adds search support into MarkDocs
        /// </summary>
        /// <param name="configuration">
        ///     Configuration builder
        /// </param>
        /// <param name="searchEngine">
        ///     Search engine
        /// </param>
        [PublicAPI]
        public static void AddSearch(
            [NotNull] this MarkDocsExtensionConfigurationBuilder configuration,
            [NotNull] ISearchEngine searchEngine)
        {
            AddSearch(configuration, _ => searchEngine);
        }
    }
}