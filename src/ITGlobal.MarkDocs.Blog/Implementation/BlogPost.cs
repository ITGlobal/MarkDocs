using System;
using System.Collections.Generic;
using System.IO;
using ITGlobal.MarkDocs.Content;
using ITGlobal.MarkDocs.Tags;

namespace ITGlobal.MarkDocs.Blog.Implementation
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
                if (resources.TryGetValue(titleImageId, out var titleImage))
                {
                    TitleImage = titleImage;
                }
                else
                {
                    var fullTitleImageId = Path.Combine(Path.GetDirectoryName(page.PageTreeNode.RelativeFilePath), titleImageId);
                    fullTitleImageId = "/" + fullTitleImageId;
                    fullTitleImageId = fullTitleImageId.Replace("\\", "/");

                    if (resources.TryGetValue(fullTitleImageId, out titleImage))
                    {
                        TitleImage = titleImage;
                    }
                    else
                    {
                        report.ForPage(page).Error($"Bad title image reference: \"{titleImageId}\"");
                    }
                }
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
        public string FileName => Page.FileName;

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
        public IReadOnlyDictionary<string, string> Anchors => Page.Anchors;

        /// <summary>
        ///     Blog post tags
        /// </summary>
        public IReadOnlyList<string> Tags { get; }

        /// <summary>
        ///     Blog post meta tags (HTML)
        /// </summary>
        public IReadOnlyList<string> MetaTags => Page.Metadata.MetaTags;

        /// <summary>
        ///     Reads blog post source markup
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        public Stream ReadMarkup() => Page.ReadMarkup();

        /// <summary>
        ///     Reads blog post rendered HTML
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        public Stream ReadHtml() => Page.ReadHtml();
    }
}