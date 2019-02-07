using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post index (per day)
    /// </summary>
    [PublicAPI]
    public interface IBlogIndexDay
    {
        /// <summary>
        ///     Date
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets a blog post by index
        /// </summary>
        [CanBeNull]
        IBlogPost this[int index] { get; }

        /// <summary>
        ///     Gets a list of blog posts
        /// </summary>
        [NotNull]
        IReadOnlyList<IBlogPost> Posts { get; }
    }
}