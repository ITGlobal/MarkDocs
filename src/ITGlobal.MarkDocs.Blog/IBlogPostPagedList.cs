using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     A list of <see cref="IBlogPost"/> with pagination support
    /// </summary>
    [PublicAPI]
    public interface IBlogPostPagedList
    {

        /// <summary>
        ///     Page count
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [NotNull]
        IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize);

    }
}