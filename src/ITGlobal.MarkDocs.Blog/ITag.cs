using JetBrains.Annotations;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Post tag with count
    /// </summary>
    [PublicAPI]
    public interface ITag
    {
        /// <summary>
        ///     Tag name
        /// </summary>
        [NotNull]
        string Name { get; }

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