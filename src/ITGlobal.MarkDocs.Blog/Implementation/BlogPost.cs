using System;
using System.Collections.Generic;
using System.IO;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Tags;

namespace ITGlobal.MarkDocs.Blog.Implementation
{
    /// <summary>
    ///     Blog post
    /// </summary>
    internal sealed class BlogPost : IBlogPost
    {
        public BlogPost(IBlogEngine engine, IPage page, DateTime date, string slug)
        {
            Engine = engine;
            Page = page;
            Date = date;
            Slug = slug;

            if (!string.IsNullOrEmpty(slug))
            {
                Id = $"/{date.Year:D04}/{date.Month:D02}/{date.Day:D02}/{slug.ToLowerInvariant()}";
            }
            else
            {
                Id = $"/{date.Year:D04}/{date.Month:D02}/{date.Day:D02}";
            }

            Tags = Page.GetPageTags();
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
        ///     Blog post date
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        ///     Blog post title
        /// </summary>
        public string Title => Page.Title;

        /// <summary>
        ///     Blog post permalink
        /// </summary>
        public IBlogPermalink Permalink { get; }

        /// <summary>
        ///     Blog post anchors (with names)
        /// </summary>
        public IReadOnlyDictionary<string, string> Anchors => Page.Anchors;

        /// <summary>
        ///     Blog post tags
        /// </summary>
        public IReadOnlyList<string> Tags { get; }

        /// <summary>
        ///     Blog post meta tags
        /// </summary>
        public IReadOnlyList<MetaTag> MetaTags => Page.Metadata.MetaTags;

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