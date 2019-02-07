using System;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     Blog post index (per day)
    /// </summary>
    internal sealed class BlogIndexDay : IBlogIndexDay
    {
        private readonly List<IBlogPost> _posts = new List<IBlogPost>();

        public BlogIndexDay(DateTime date)
        {
            Date = date;
        }

        /// <summary>
        ///     Date
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        public int Count => _posts.Count;

        /// <summary>
        ///     Gets a blog post by index
        /// </summary>
        public IBlogPost this[int index] => (index >= 0 && index < _posts.Count) ? _posts[index] : null;

        /// <summary>
        ///     Gets a list of blog posts
        /// </summary>
        public IReadOnlyList<IBlogPost> Posts => _posts;

        public void Add(BlogPost post) => _posts.Add(post);
    }
}