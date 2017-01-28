using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     Helper methods for search service
    /// </summary>
    [PublicAPI]
    public static class SearchExtensionMethods
    {
        /// <summary>
        ///     Gets an instance of search extension
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <returns>
        ///     Search service
        /// </returns>
        [PublicAPI, NotNull]
        public static ISearchService GetSearchService([NotNull] this IDocumentation documentation)
        {
            var extension = documentation.Service.GetExtension<SearchExtension>();
            if (extension == null)
            {
                throw new InvalidOperationException($"{typeof(SearchExtension)} is not registered");
            }

            return extension.GetSearchService(documentation);
        }

        /// <summary>
        ///     Search documentation
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     Search results
        /// </returns>
        [PublicAPI, NotNull]
        public static IReadOnlyList<SearchResultItem> Search([NotNull] this IDocumentation documentation, [NotNull] string query)
        {
            var service = documentation.GetSearchService();
            return service.Search(query);
        }

        /// <summary>
        ///     Gets a list of search query suggestions
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     List of search suggestions
        /// </returns>
        [PublicAPI, NotNull]
        public static IReadOnlyList<string> Suggest([NotNull] this IDocumentation documentation, [NotNull] string query)
        {
            var service = documentation.GetSearchService();
            return service.Suggest(query);
        }
    }
}