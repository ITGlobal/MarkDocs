using System;
using System.Collections.Generic;
using System.Linq;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     Blog post index (per month)
    /// </summary>
    internal sealed class BlogIndexMonth : IBlogIndexMonth
    {
        private readonly Dictionary<int, IBlogIndexDay> _byDay
            = new Dictionary<int, IBlogIndexDay>();

        public BlogIndexMonth(int year, int month)
        {
            Year = year;
            Month = month;
        }

        /// <summary>
        ///     Year number
        /// </summary>
        public int Year { get; }

        /// <summary>
        ///     Month number
        /// </summary>
        public int Month { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets a blog post index by day
        /// </summary>
        public IBlogIndexDay this[int day]
        {
            get
            {
                if (!_byDay.TryGetValue(day, out var index))
                {
                    return new EmptyBlogIndexDay(new DateTime(Year, Month, day));
                }
                return index;
            }
        }

        /// <summary>
        ///     Gets a blog post index by day
        /// </summary>
        public IReadOnlyDictionary<int, IBlogIndexDay> Days => _byDay;

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        public IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize)
        {
            var query = Days
                .OrderByDescending(_ => _.Key)
                .SelectMany(_ => _.Value.Posts)
                .Cast<BlogPost>()
                .OrderByDescending(_ => _.Date)
                .ThenByDescending(_ => _.Slug)
                .ThenByDescending(_ => _.ContentId)
                .Cast<IBlogPost>();

            var list = query.Skip(page * pageSize).Take(pageSize).ToList();
            return list;
        }

        public void Add(BlogPost post)
        {
            Count++;
            if (!_byDay.TryGetValue(post.Date.Day, out var index))
            {
                index = new BlogIndexDay(post.Date);
                _byDay.Add(post.Date.Day, index);
            }

            ((BlogIndexDay)index).Add(post);
        }
    }
}