using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog.Example.Models
{
    public sealed class ListModel
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyList<IBlogPost> Posts { get; set; }
    }
}
