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

    /// <summary>
    ///     A cache item
    /// </summary>
    [PublicAPI]
    public interface ICacheItem
    {
        /// <summary>
        ///     Cache item ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        [PublicAPI, NotNull]
        IDocumentation Documentation { get; }

        /// <summary>
        ///     Cache item type
        /// </summary>
        [PublicAPI]
        CacheItemType Type { get; }
    }

    /// <summary>
    ///     Cache item type
    /// </summary>
    [PublicAPI]
    public enum CacheItemType
    {
        /// <summary>
        ///     Cached page
        /// </summary>
        [PublicAPI]
        Page,

        /// <summary>
        ///     Cached attachment
        /// </summary>
        [PublicAPI]
        Attachment
    }
}