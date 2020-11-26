using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ITGlobal.MarkDocs.Source;
using ITGlobal.MarkDocs.Tags;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class BlogIndex : IBlogIndex
    {

        private readonly IDocumentation _documentation;

        private readonly ImmutableDictionary<int, IBlogIndexYear> _byYear;

        private readonly ImmutableDictionary<string, ITag> _tags;

        private readonly ImmutableList<IBlogPost> _postsList;

        public BlogIndex(IBlogEngine engine, IDocumentation documentation, ICompilationReportBuilder report)
        {
            _documentation = documentation;

            // Collect resources
            foreach (var attachment in documentation.Files.Values)
            {
                if (Resources.ContainsKey(attachment.Id))
                {
                    report.Error($"Non-unique resource: \"{attachment.Id}\"");
                    continue;
                }

                Resources.Add(attachment.Id, new BlogAttachment(engine, attachment));
            }

            // Collect posts
            var byYear = new Dictionary<int, IBlogIndexYear>();
            foreach (var page in documentation.RootPage.NestedPages)
            {
                if (!BlogPostMetadataProvider.TryParse(page, report, out var date, out var slug))
                {
                    continue;
                }

                var post = new BlogPost(engine, page, report, date, slug, Resources);
                if (Resources.ContainsKey(post.Id))
                {
                    report.Error($"Non-unique resource: \"{post.Id}\"");
                    continue;
                }

                Resources.Add(post.Id, post);
                Posts.Add(post.Id, post);

                if (!byYear.TryGetValue(post.Date.Year, out var yearIndex))
                {
                    yearIndex = new BlogIndexYear(post.Date.Year);
                    byYear.Add(post.Date.Year, yearIndex);
                }

                ((BlogIndexYear) yearIndex).Add(post);
            }

            _byYear = byYear.ToImmutableDictionary();

            // Collect permalinks
            foreach (var post in Posts.Values)
            {
                var address = post.Page.Metadata.GetString("permalink");
                if (!string.IsNullOrEmpty(address))
                {
                    address = "/" + address;
                    if (Resources.ContainsKey(address))
                    {
                        report.Error($"Non-unique resource: \"{post.Id}\" (permalink to \"{post.Id}\"");
                        continue;
                    }

                    var permalink = new BlogPermalink(post, address);
                    Resources.Add(permalink.Id, permalink);
                    post.Permalink = permalink;
                }
            }

            // Collect tags
            Tags = documentation.GetTags()
                .Select(tag => new Tag(documentation, tag, SelectPost))
                .OrderByDescending(_ => _.Count)
                .ThenBy(_ => _.Name)
                .ToArray();
            var tags = new Dictionary<string, ITag>(StringComparer.OrdinalIgnoreCase);
            foreach (var tag in Tags)
            {
                tags[tag.Name] = tag;
            }

            _tags = tags.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);

            // Build flat post list
            _postsList = Posts.Values
                .OrderByDescending(_ => _.Date)
                .ThenByDescending(_ => _.Slug)
                .ThenByDescending(_ => _.ContentId)
                .Cast<IBlogPost>()
                .ToImmutableList();

            BlogPost SelectPost(string id)
            {
                Posts.TryGetValue(id, out var post);
                return post;
            }
        }

        public Dictionary<string, IBlogResource> Resources { get; } =
            new Dictionary<string, IBlogResource>(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, BlogPost> Posts { get; } =
            new Dictionary<string, BlogPost>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     Gets a blog post index by year
        /// </summary>
        public IBlogIndexYear this[int year]
        {
            get
            {
                if (!Years.TryGetValue(year, out var value))
                {
                    return new EmptyBlogIndexYear(year);
                }

                return value;
            }
        }

        /// <summary>
        ///     Gets a blog post index by year
        /// </summary>
        public IReadOnlyDictionary<int, IBlogIndexYear> Years => _byYear;

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        public int Count => Posts.Count;

        /// <summary>
        ///     Gets blog tags
        /// </summary>
        public IReadOnlyList<ITag> Tags { get; }

        /// <summary>
        ///     Gets pages by tag
        /// </summary>
        public ITag Tag(string name)
        {
            if (!_tags.TryGetValue(name, out var tag))
            {
                return new EmptyTag(name);
            }

            return tag;
        }

        /// <summary>
        ///     Gets pages by tags
        /// </summary>
        public IBlogPostPagedList Query(string[] includeTags = null, string[] excludeTags = null)
        {
            var pages = _documentation.GetPagesByTags(includeTags, excludeTags);
            var posts = new BlogPostPagedList(
                pages
                    .Select(_ => SelectPost(_.Id))
                    .Where(_ => _ != null)
                    .OrderByDescending(_ => _.Date)
                    .ThenByDescending(_ => _.Slug)
                    .ThenByDescending(_ => _.ContentId)
            );
            return posts;

            BlogPost SelectPost(string id)
            {
                Posts.TryGetValue(id, out var post);
                return post;
            }
        }

        /// <summary>
        ///     Gets pages by tag
        /// </summary>
        public IBlogPostPagedList WithTag(string name)
        {
            return Query(includeTags: new[] {name});
        }

        /// <summary>
        ///     Gets pages without tag
        /// </summary>
        public IBlogPostPagedList WithoutTag(string name)
        {
            return Query(excludeTags: new[] {name});
        }

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        public IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize)
            => _postsList.Skip(page * pageSize).Take(pageSize).ToArray();

    }
}