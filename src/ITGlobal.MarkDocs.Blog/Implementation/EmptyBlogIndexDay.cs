using System;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog.Implementation
{
    internal sealed class EmptyBlogIndexDay : IBlogIndexDay
    {
        private static readonly IReadOnlyList<IBlogPost> EmptyList = new IBlogPost[0];

        public EmptyBlogIndexDay(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; }
        public int Count => 0;

        public IBlogPost this[int index] => null;

        public IReadOnlyList<IBlogPost> Posts => EmptyList;
    }
}