using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     Search service
    /// </summary>
    [PublicAPI]
    public interface ISearchService
    {
        /// <summary>
        ///     Search documentation
        /// </summary>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     Search results
        /// </returns>
        [PublicAPI, NotNull]
        IReadOnlyList<SearchResultItem> Search([NotNull] string query);

        /// <summary>
        ///     Gets a list of search query suggestions
        /// </summary>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     List of search suggestions
        /// </returns>
        [PublicAPI, NotNull]
        IReadOnlyList<string> Suggest([NotNull] string query);
    }
}
