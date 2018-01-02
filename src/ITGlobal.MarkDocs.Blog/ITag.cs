using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Post tag with count
    /// </summary>
    public interface ITag
    {
        /// <summary>
        ///     Tag name
        /// </summary>
        [PublicAPI, NotNull]
        string Name { get; }

        /// <summary>
        ///     Page count
        /// </summary>
        [PublicAPI]
        int Count { get; }
        
        /// <summary>
        ///     Gets a blog posts paged list
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize);
    }
}