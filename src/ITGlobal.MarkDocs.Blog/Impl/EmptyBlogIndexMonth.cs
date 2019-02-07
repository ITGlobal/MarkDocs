using System;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class EmptyBlogIndexMonth : IBlogIndexMonth
    {
        private static readonly IReadOnlyList<IBlogPost> EmptyList = new IBlogPost[0];
        private static readonly IReadOnlyDictionary<int, IBlogIndexDay> EmptyDictionary = new Dictionary<int, IBlogIndexDay>();

        public EmptyBlogIndexMonth(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public int Year { get; }
        public int Month { get; }
        public int Count => 0;

        public IBlogIndexDay this[int day] => new EmptyBlogIndexDay(new DateTime(Year, Month, day));

        public IReadOnlyDictionary<int, IBlogIndexDay> Days => EmptyDictionary;

        public IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize) => EmptyList;
    }
}