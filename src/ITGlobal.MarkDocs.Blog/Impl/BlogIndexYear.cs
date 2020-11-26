using System.Collections.Generic;
using System.Linq;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     Blog post index (per year)
    /// </summary>
    internal sealed class BlogIndexYear : IBlogIndexYear
    {
        private readonly Dictionary<int, IBlogIndexMonth> _byMonth
            = new Dictionary<int, IBlogIndexMonth>();
        
        public BlogIndexYear(int year)
        {
            Year = year;
        }

        /// <summary>
        ///     Year number
        /// </summary>
        public int Year { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        ///     Gets a blog post index by month
        /// </summary>
        public IBlogIndexMonth this[int month]
        {
            get
            {
                if (!_byMonth.TryGetValue(month, out var index))
                {
                    return new EmptyBlogIndexMonth(Year, month);
                }
                return index;
            }
        }

        /// <summary>
        ///     Gets a blog post index by month
        /// </summary>
        public IReadOnlyDictionary<int, IBlogIndexMonth> Months => _byMonth;

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        public IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize)
        {
            var query = Months
                .OrderByDescending(_ => _.Key)
                .SelectMany(_ => _.Value.Days)
                .OrderByDescending(_ => _.Key)
                .SelectMany(_=>_.Value.Posts)
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
            if (!_byMonth.TryGetValue(post.Date.Month, out var index))
            {
                index = new BlogIndexMonth(Year, post.Date.Month);
                _byMonth.Add(post.Date.Month, index);
            }

            ((BlogIndexMonth)index).Add(post);
        }
    }
}