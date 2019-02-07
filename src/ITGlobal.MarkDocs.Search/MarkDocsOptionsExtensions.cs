using JetBrains.Annotations;
using ITGlobal.MarkDocs.Search.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsOptions"/> to configure search extension
    /// </summary>
    [PublicAPI]
    public static class MarkDocsOptionsExtensions
    {
        /// <summary>
        ///     Adds search support into MarkDocs
        /// </summary>
        /// <param name="options">
        ///     Configuration builder
        /// </param>
        /// <param name="indexDirectory">
        ///     Path to lucene index directory
        /// </param>
        [NotNull]
        public static MarkDocsOptions AddSearch(
            [NotNull] this MarkDocsOptions options,
            [NotNull] string indexDirectory)
        {
           return AddSearch(options, new SearchOptions { IndexDirectory = indexDirectory });
        }

        /// <summary>
        ///     Adds search support into MarkDocs
        /// </summary>
        /// <param name="options">
        ///     Configuration builder
        /// </param>
        /// <param name="opt">
        ///     Options for search extension
        /// </param>
        [PublicAPI]
        public static MarkDocsOptions AddSearch(
            [NotNull] this MarkDocsOptions options,
            [NotNull] SearchOptions opt)
        {
            options.ConfigureServices(_ => _.AddSingleton(opt));
            options.ConfigureServices(_ => _.AddSingleton< LuceneSearchEngine>());
            options.ConfigureServices(_ => _.AddSingleton<SearchExtensionFactory>());
            options.AddExtension(_ => _.GetRequiredService<SearchExtensionFactory>());
            return options;
        }
    }
}
