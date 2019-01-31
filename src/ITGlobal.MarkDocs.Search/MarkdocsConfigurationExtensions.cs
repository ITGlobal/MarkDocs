using JetBrains.Annotations;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Search.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsExtensionOptions"/> to configure search extension
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
            [NotNull] this MarkDocsExtensionOptions configuration,
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
            [NotNull] this MarkDocsExtensionOptions configuration,
            [NotNull] SearchOptions options)
        {
            configuration.ConfigureServices(_ => _.AddSingleton(options));
            configuration.ConfigureServices(_ => _.AddSingleton< LuceneSearchEngine>());
            configuration.Use<SearchExtensionFactory>();
        }
    }
}
