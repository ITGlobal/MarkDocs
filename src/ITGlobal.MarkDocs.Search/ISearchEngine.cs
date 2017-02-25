using System.Collections.Generic;
using ITGlobal.MarkDocs.Extensions;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     Search engine
    /// </summary>
    public interface ISearchEngine
    {
        /// <summary>
        ///     Runs indexing on a documentation state
        /// </summary>
        void Index([NotNull] IMarkDocServiceState state, bool isInitial);

        /// <summary>
        ///     Gets a list of search suggestions
        /// </summary>
        [NotNull]
        IReadOnlyList<string> Suggest(IDocumentation documentation, [NotNull] string query);

        /// <summary>
        ///     Runs a search
        /// </summary>
        [NotNull]
        IReadOnlyList<SearchResultItem> Search(IDocumentation documentation, [NotNull] string query);
    }
}