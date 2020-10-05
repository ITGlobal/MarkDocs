using System.Collections.Generic;
using System.Linq;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     A list of <see cref="IBlogPost"/> with pagination support
    /// </summary>
    internal class BlogPostPagedList : IBlogPostPagedList
    {
        private readonly List<IBlogPost> _posts;

        public BlogPostPagedList(IEnumerable<IBlogPost> posts)
        {
            _posts = posts.ToList();
        }

        /// <summary>
        ///     Page count
        /// </summary>
        public int Count => _posts.Count;

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        public IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize)
            => _posts.Skip(page * pageSize).Take(pageSize).ToArray();
    }
}