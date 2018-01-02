using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post index (per month)
    /// </summary>
    public interface IBlogIndexMonth
    {
        /// <summary>
        ///     Year number
        /// </summary>
        [PublicAPI]
        int Year { get; }

        /// <summary>
        ///     Month number
        /// </summary>
        [PublicAPI]
        int Month { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        [PublicAPI]
        int Count { get; }

        /// <summary>
        ///     Gets a blog post index by day
        /// </summary>
        [PublicAPI, NotNull]
        IBlogIndexDay this[int day] { get; }

        /// <summary>
        ///     Gets a blog post index by day
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyDictionary<int, IBlogIndexDay> Days { get; }

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize);
    }
}