using JetBrains.Annotations;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post index (per month)
    /// </summary>
    [PublicAPI]
    public interface IBlogIndexMonth
    {
        /// <summary>
        ///     Year number
        /// </summary>
        int Year { get; }

        /// <summary>
        ///     Month number
        /// </summary>
        int Month { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets a blog post index by day
        /// </summary>
        [NotNull]
        IBlogIndexDay this[int day] { get; }

        /// <summary>
        ///     Gets a blog post index by day
        /// </summary>
        [NotNull]
        IReadOnlyDictionary<int, IBlogIndexDay> Days { get; }

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [NotNull]
        IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize);
    }
}