using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post index (root)
    /// </summary>
    public interface IBlogIndex
    {
        /// <summary>
        ///     Gets a blog post index by year
        /// </summary>
        [PublicAPI, NotNull]
        IBlogIndexYear this[int year] { get; }

        /// <summary>
        ///     Gets a blog post index by year
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyDictionary<int, IBlogIndexYear> Years { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        [PublicAPI]
        int Count { get; }

        /// <summary>
        ///     Gets blog tags
        /// </summary>
        [PublicAPI]
        IReadOnlyList<ITag> Tags { get; }

        /// <summary>
        ///     Gets pages by tag
        /// </summary>
        [PublicAPI, NotNull]
        ITag Tag([NotNull] string name);

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize);
    }
}