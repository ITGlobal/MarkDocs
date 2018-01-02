using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog.Example.Models
{
    public sealed class SearchModel
    {
        public string Query { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyList<ITextSearchResultItem> Items { get; set; }
    }
}