using System.Collections.Generic;
using ITGlobal.MarkDocs.Search;

namespace ITGlobal.MarkDocs.Tools.Serve.Models
{
    public sealed class SearchPageModel
    {
        public SearchPageModel(IDocumentation documentation)
        {
            Documentation = documentation;
        }

        public SearchPageModel(
            IDocumentation documentation,
            string query,
            IReadOnlyList<SearchResultItem> searchResults,
            IReadOnlyList<string> suggestions)
        {
            Documentation = documentation;
            Query = query;
            SearchResults = searchResults;
            Suggestions = suggestions;
        }

        public IDocumentation Documentation { get; }
        public string Query { get; }
        public IReadOnlyList<SearchResultItem> SearchResults { get; }
        public bool HasSearchResults => SearchResults != null;
        public IReadOnlyList<string> Suggestions { get; }
        public bool HasSuggestions => Suggestions != null && Suggestions.Count > 0;
    }
}