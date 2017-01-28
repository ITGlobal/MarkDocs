using System.Collections.Generic;
using ITGlobal.MarkDocs.Search;

namespace ITGlobal.MarkDocs.Example.Model
{
    public sealed class SearchResultsModel
    {
        public SearchResultsModel(IDocumentation documentation, IReadOnlyList<SearchResultItem> items, string query)
        {
            Documentation = documentation;
            Items = items;
            Query = query;
        }

        public IDocumentation Documentation { get; }
        public IReadOnlyList<SearchResultItem> Items { get; }
        public string Query { get; }
    }
}