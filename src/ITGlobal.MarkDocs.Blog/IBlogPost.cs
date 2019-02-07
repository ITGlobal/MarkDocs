using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post
    /// </summary>
    [PublicAPI]
    public interface IBlogPost : IBlogResource
    {
        /// <summary>
        ///     Blog post permanent identifier
        /// </summary>
        [CanBeNull]
        string ContentId { get; }

        /// <summary>
        ///     Blog post author
        /// </summary>
        [CanBeNull]
        string Author { get; }

        /// <summary>
        ///     Blog post date
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        ///     Blog post title
        /// </summary>
        [NotNull]
        string Title { get; }

        /// <summary>
        ///     Blog post description
        /// </summary>
        [CanBeNull]
        string Description { get; }

        /// <summary>
        ///     A post's title image if specified
        /// </summary>
        [CanBeNull]
        IBlogResource TitleImage { get; }

        /// <summary>
        ///     Blog post permalink
        /// </summary>
        [CanBeNull]
        IBlogPermalink Permalink { get; }

        /// <summary>
        ///     Blog post anchors (with names)
        /// </summary>
        [NotNull]
        PageAnchors Anchors { get; }

        /// <summary>
        ///     Blog post tags
        /// </summary>
        [NotNull]
        IReadOnlyList<string> Tags { get; }

        /// <summary>
        ///     Blog post meta tags (HTML)
        /// </summary>
        [NotNull]
        IReadOnlyList<string> MetaTags { get; }

        /// <summary>
        ///     true if blog post has a preview
        /// </summary>
        [PublicAPI]
        bool HasPreview { get; }

        /// <summary>
        ///     Reads blog post rendered HTML
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        [NotNull]
        Stream ReadHtml();

        /// <summary>
        ///     Reads blog post preview
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        [NotNull]
        Stream ReadPreviewHtml();
    }
}