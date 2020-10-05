using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Tags;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     Post tag with count
    /// </summary>
    internal sealed class Tag : BlogPostPagedList, ITag
    {
        public Tag(IDocumentation documentation, string name, Func<string, BlogPost> postSelector)
            : base(GetBlogPosts(documentation, name, postSelector))
        {
            Name = name;
        }

        private static IEnumerable<IBlogPost> GetBlogPosts(
            IDocumentation documentation,
            string name,
            Func<string, BlogPost> postSelector)
        {
            return documentation.GetPagesByTag(name)
                .Select(_ => postSelector(_.Id))
                .Where(_ => _ != null)
                .OrderByDescending(_ => _.Date)
                .ThenByDescending(_ => _.Slug)
                .ThenByDescending(_ => _.ContentId)
                .Cast<IBlogPost>();
        }

        /// <summary>
        ///     Tag name
        /// </summary>
        public string Name { get; }
    }
}