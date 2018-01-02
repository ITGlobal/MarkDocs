using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Tags;

namespace ITGlobal.MarkDocs.Blog.Implementation
{
    internal sealed class BlogIndex : IBlogIndex
    {
        private readonly Dictionary<int, IBlogIndexYear> _byYear
            = new Dictionary<int, IBlogIndexYear>();
        private readonly Dictionary<string, ITag> _tags
            = new Dictionary<string, ITag>(StringComparer.OrdinalIgnoreCase);

        private readonly List<IBlogPost> _postsList;

        public BlogIndex(IBlogEngine engine, IDocumentation documentation, BlogCompilationReport report)
        {
            // Collect resources
            foreach (var attachment in documentation.Attachments)
            {
                if (Resources.ContainsKey(attachment.Id))
                {
                    report.AddError($"Non-unique resource: \"{attachment.Id}\"");
                    continue;
                }

                Resources.Add(attachment.Id, new BlogAttachment(engine, attachment));
            }

            // Collect posts
            foreach (var node in documentation.PageTree.Pages)
            {
                var page = documentation.GetPage(node.Id);
                if (page != null)
                {
                    if (!BlogPostMetadataProvider.TryParse(page, report, out var date, out var slug))
                    {
                        continue;
                    }

                    var post = new BlogPost(engine, page, date, slug);
                    if (Resources.ContainsKey(post.Id))
                    {
                        report.AddError($"Non-unique resource: \"{post.Id}\"");
                        continue;
                    }

                    Resources.Add(post.Id, post);
                    Posts.Add(post.Id, post);

                    if (!_byYear.TryGetValue(post.Date.Year, out var yearIndex))
                    {
                        yearIndex = new BlogIndexYear(post.Date.Year);
                        _byYear.Add(post.Date.Year, yearIndex);
                    }

                    ((BlogIndexYear)yearIndex).Add(post);
                }
            }

            // Collect permalinks
            foreach (var post in Posts.Values)
            {
                // TODO BlogPermalink
            }

            // Collect tags
            Tags = documentation.GetTags()
                .Select(tag => new Tag(documentation, tag, SelectPost))
                .OrderByDescending(_ => _.Count)
                .ThenBy(_ => _.Name)
                .ToArray();
            foreach (var tag in Tags)
            {
                _tags[tag.Name] = tag;
            }

            BlogPost SelectPost(string id)
            {
                Posts.TryGetValue(id, out var post);
                return post;
            }

            // Build flat post list
            _postsList = Posts.Values
                .OrderByDescending(_ => _.Date)
                .ThenByDescending(_ => _.Slug)
                .ThenByDescending(_ => _.ContentId)
                .Cast<IBlogPost>()
                .ToList();
        }

        public Dictionary<string, IBlogResource> Resources { get; } = new Dictionary<string, IBlogResource>(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, BlogPost> Posts { get; } = new Dictionary<string, BlogPost>(StringComparer.OrdinalIgnoreCase);

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
        ///     Gets a blog posts paged list
        /// </summary>
        public IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize)
            => _postsList.Skip(page * pageSize).Take(pageSize).ToArray();
    }
}