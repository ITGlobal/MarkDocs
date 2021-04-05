using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITGlobal.MarkDocs.Impl;
using ITGlobal.MarkDocs.Source;
using ITGlobal.MarkDocs.Tags;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     Blog post
    /// </summary>
    internal sealed class BlogPost : IBlogPost
    {

        public BlogPost(
            IBlogEngine engine,
            IPage page,
            ICompilationReportBuilder report,
            DateTime date,
            string slug,
            Dictionary<string, IBlogResource> resources)
        {
            Engine = engine;
            Page = page;
            Date = date;
            Slug = slug;

            // Evaluate page ID
            if (!string.IsNullOrEmpty(slug))
            {
                Id = $"/{date.Year:D04}/{date.Month:D02}/{date.Day:D02}/{slug.ToLowerInvariant()}";
            }
            else
            {
                Id = $"/{date.Year:D04}/{date.Month:D02}/{date.Day:D02}";
            }

            // Evaluate tags
            Tags = page.GetPageTags();

            // Get title image
            var titleImageId = page.Metadata.GetString("title_image") ?? page.Metadata.GetString("image");
            if (titleImageId != null)
            {
                IBlogResource titleImage = null;
                foreach (var fullTitleImageId in EnumerateResourceIdCandidates(titleImageId))
                {
                    if (resources.TryGetValue(fullTitleImageId, out var id))
                    {
                        titleImage = id;
                        break;
                    }
                }

                if (titleImage != null)
                {
                    TitleImage = titleImage;
                }
                else
                {
                    report.Error(page.RelativePath, $"Bad title image reference: \"{titleImageId}\"");
                }
            }

            IEnumerable<string> EnumerateResourceIdCandidates(string id)
            {
                var fullId1 = id;
                ResourceId.Normalize(ref fullId1);
                yield return fullId1;

                var fullId2 = Path.Combine(page.RelativePath, id);
                ResourceId.Normalize(ref fullId2);
                yield return fullId2;

                var fullId3 = Path.Combine(Path.GetDirectoryName(page.RelativePath), id);
                ResourceId.Normalize(ref fullId3);
                yield return fullId3;
            }
        }

        public IPage Page { get; }
        public string Slug { get; }

        /// <summary>
        ///     Resource ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     A reference to a blog engine
        /// </summary>
        public IBlogEngine Engine { get; }

        /// <summary>
        ///     Name of resource file with extension (only name, not a full path)
        /// </summary>
        public string RelativePath => Page.RelativePath;

        /// <summary>
        ///     Resource type
        /// </summary>
        public BlogResourceType Type => BlogResourceType.Post;

        /// <summary>
        ///     Blog post permanent identifier
        /// </summary>
        public string ContentId => Page.Metadata.ContentId;

        /// <summary>
        ///     Blog post author
        /// </summary>
        public string Author => Page.Metadata.LastChangedBy;

        /// <summary>
        ///     Blog post date
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        ///     Blog post title
        /// </summary>
        public string Title => Page.Title;

        /// <summary>
        ///     Blog post description
        /// </summary>
        public string Description => Page.Description;

        /// <summary>
        ///     A post's title image if specified
        /// </summary>
        public IBlogResource TitleImage { get; }

        /// <summary>
        ///     Blog post permalink
        /// </summary>
        public IBlogPermalink Permalink { get; set; }

        /// <summary>
        ///     Blog post anchors (with names)
        /// </summary>
        public PageAnchors Anchors => Page.Anchors;

        /// <summary>
        ///     Blog post tags
        /// </summary>
        public IReadOnlyList<string> Tags { get; }

        /// <summary>
        ///     Blog post meta tags (HTML)
        /// </summary>
        public IReadOnlyList<string> MetaTags => Page.Metadata.MetaTags;

        /// <summary>
        ///     true if blog post has a preview
        /// </summary>
        public bool HasPreview => Page.Preview != null;

        /// <summary>
        ///     Reads blog post rendered HTML
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        public Stream ReadHtml() => Page.OpenRead();

        /// <summary>
        ///     Reads blog post preview
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        public Stream ReadPreviewHtml() => Page.Preview.OpenRead();

    }
}