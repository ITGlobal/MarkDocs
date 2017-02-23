using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
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
        Attachment,

        /// <summary>
        ///     Cached illustration
        /// </summary>
        [PublicAPI]
        Illustration
    }
}