using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Configuration options for disk-based cache
    /// </summary>
    [PublicAPI]
    public sealed class DiskCacheOptions
    {
        /// <summary>
        ///     A path to cache directory
        /// </summary>
        [PublicAPI, NotNull]
        public string Directory { get; set; }

        /// <summary>
        ///     Enable concurrent page cache writes
        /// </summary>
        [PublicAPI]
        public bool EnableConcurrentWrites { get; set; } = true;
    }
}