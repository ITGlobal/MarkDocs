using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Tags;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     Post tag with count
    /// </summary>
    internal sealed class Tag : ITag
    {
        private readonly List<IBlogPost> _posts;

        public Tag(IDocumentation documentation, string name, Func<string, BlogPost> postSelector)
        {
            name = (name ?? "").ToLowerInvariant().Trim();

            Name = name;
            _posts = documentation.GetPagesByTag(name)
                .Select(_ => postSelector(_.Id))
                .Where(_ => _ != null)
                .OrderByDescending(_ => _.Date)
                .ThenByDescending(_ => _.Slug)
                .ThenByDescending(_ => _.ContentId)
                .Cast<IBlogPost>()
                .ToList();
        }

        /// <summary>
        ///     Tag name
        /// </summary>
        public string Name { get; }

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