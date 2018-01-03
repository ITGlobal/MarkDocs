using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post
    /// </summary>
    public interface IBlogPost : IBlogResource
    {
        /// <summary>
        ///     Blog post permanent identifier
        /// </summary>
        [PublicAPI, CanBeNull]
        string ContentId { get; }

        /// <summary>
        ///     Blog post author
        /// </summary>
        [PublicAPI, CanBeNull]
        string Author { get; }

        /// <summary>
        ///     Blog post date
        /// </summary>
        [PublicAPI]
        DateTime Date { get; }

        /// <summary>
        ///     Blog post title
        /// </summary>
        [PublicAPI, NotNull]
        string Title { get; }

        /// <summary>
        ///     Blog post permalink
        /// </summary>
        IBlogPermalink Permalink { get; }

        /// <summary>
        ///     Blog post anchors (with names)
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyDictionary<string, string> Anchors { get; }

        /// <summary>
        ///     Blog post tags
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<string> Tags { get; }

        /// <summary>
        ///     Blog post meta tags
        /// </summary>
        [PublicAPI]
        IReadOnlyDictionary<string, string> MetaTags { get; }

        /// <summary>
        ///     Reads blog post source markup
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        [PublicAPI, NotNull]
        Stream ReadMarkup();

        /// <summary>
        ///     Reads blog post rendered HTML
        /// </summary>
        /// <returns>
        ///     Read-only stream
        /// </returns>
        [PublicAPI, NotNull]
        Stream ReadHtml();
    }
}