using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post index (per day)
    /// </summary>
    public interface IBlogIndexDay
    {
        /// <summary>
        ///     Date
        /// </summary>
        [PublicAPI]
        DateTime Date { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        [PublicAPI]
        int Count { get; }

        /// <summary>
        ///     Gets a blog post by index
        /// </summary>
        [PublicAPI, CanBeNull]
        IBlogPost this[int index]{ get; }

        /// <summary>
        ///     Gets a list of blog posts
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IBlogPost> Posts { get; }
    }
}