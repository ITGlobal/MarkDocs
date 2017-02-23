using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Provides content caching services
    /// </summary>
    [PublicAPI]
    public interface ICache
    {
        /// <summary>
        ///     Starts update operation
        /// </summary>
        /// <returns>
        ///     Cache update operation
        /// </returns>
        [PublicAPI, NotNull]
        ICacheUpdateOperation BeginUpdate();

        /// <summary>
        ///     Gets a cached content
        /// </summary>
        /// <param name="item">
        ///     Cache item
        /// </param>
        /// <returns>
        ///     Cached content or null
        /// </returns>
        [PublicAPI, CanBeNull]
        Stream Read([NotNull] ICacheItem item);
    }
}