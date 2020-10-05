using JetBrains.Annotations;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog post index (root)
    /// </summary>
    [PublicAPI]
    public interface IBlogIndex
    {
        /// <summary>
        ///     Gets a blog post index by year
        /// </summary>
        [NotNull]
        IBlogIndexYear this[int year] { get; }

        /// <summary>
        ///     Gets a blog post index by year
        /// </summary>
        [NotNull]
        IReadOnlyDictionary<int, IBlogIndexYear> Years { get; }

        /// <summary>
        ///     Gets blog post count
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets blog tags
        /// </summary>
        [NotNull]
        IReadOnlyList<ITag> Tags { get; }

        /// <summary>
        ///     Gets tag by its name
        /// </summary>
        [NotNull]
        ITag Tag([NotNull] string name);

        /// <summary>
        ///     Gets pages by tag
        /// </summary>
        [NotNull]
        IBlogPostPagedList WithTag([NotNull] string name);

        /// <summary>
        ///     Gets pages by tags
        /// </summary>
        [NotNull]
        IBlogPostPagedList Query(string[] includeTags = null, string[] excludeTags = null);

        /// <summary>
        ///     Gets pages without tag
        /// </summary>
        [NotNull]
        IBlogPostPagedList WithoutTag([NotNull] string name);

        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [NotNull]
        IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize);
    }
}