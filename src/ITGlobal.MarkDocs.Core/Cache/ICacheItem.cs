using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
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
}