using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Cache verification result
    /// </summary>
    [PublicAPI]
    public enum CacheVerifyResult
    {
        /// <summary>
        ///     Cache is out of date
        /// </summary>
        [PublicAPI]
        OutOfDate,

        /// <summary>
        ///     Cache is up to date
        /// </summary>
        [PublicAPI]
        UpToDate
    }
}