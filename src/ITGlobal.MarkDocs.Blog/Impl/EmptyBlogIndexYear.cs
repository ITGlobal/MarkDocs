using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class EmptyBlogIndexYear : IBlogIndexYear
    {
        private static readonly IReadOnlyList<IBlogPost> EmptyList = new IBlogPost[0];
        private static readonly IReadOnlyDictionary<int, IBlogIndexMonth> EmptyDictionary = new Dictionary<int, IBlogIndexMonth>();

        public EmptyBlogIndexYear(int year)
        {
            Year = year;
        }

        public int Year { get; }
        public int Count => 0;

        public IBlogIndexMonth this[int month] => new EmptyBlogIndexMonth(Year, month);

        public IReadOnlyDictionary<int, IBlogIndexMonth> Months => EmptyDictionary;

        public IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize) => EmptyList;
    }
}