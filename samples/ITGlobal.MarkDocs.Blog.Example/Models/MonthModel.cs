using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog.Example.Models
{
    public sealed class MonthModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public IReadOnlyList<IBlogPost> Posts { get; set; }
    }
}