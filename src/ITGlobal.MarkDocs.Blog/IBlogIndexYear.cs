using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post index (per year)
    /// </summary>
    public interface IBlogIndexYear
    {
        /// <summary>
        ///     Year number
        /// </summary>
        [PublicAPI]
        int Year { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        [PublicAPI]
        int Count { get; }

        /// <summary>
        ///     Gets a blog post index by month
        /// </summary>
        [PublicAPI, NotNull]
        IBlogIndexMonth this[int month] { get; }

        /// <summary>
        ///     Gets a blog post index by month
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyDictionary<int, IBlogIndexMonth> Months { get; }

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize);
    }
}