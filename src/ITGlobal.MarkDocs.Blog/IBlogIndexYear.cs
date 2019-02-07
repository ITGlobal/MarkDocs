using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post index (per year)
    /// </summary>
    [PublicAPI]
    public interface IBlogIndexYear
    {
        /// <summary>
        ///     Year number
        /// </summary>
        int Year { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets a blog post index by month
        /// </summary>
        [NotNull]
        IBlogIndexMonth this[int month] { get; }

        /// <summary>
        ///     Gets a blog post index by month
        /// </summary>
        [NotNull]
        IReadOnlyDictionary<int, IBlogIndexMonth> Months { get; }

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [NotNull]
        IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize);
    }
}