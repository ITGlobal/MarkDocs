using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsConfigurationBuilder"/> to configure cache
    /// </summary>
    [PublicAPI]
    public static class CacheConfigurationExtensions
    {
        /// <summary>
        ///     Configures <paramref name="config"/> to disable content caching
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsCacheConfigurationBuilder UseNull([NotNull] this MarkDocsCacheConfigurationBuilder config)
        {
            config.Use(services => new NullCache());
            return config;
        }

        /// <summary>
        ///     Configures <paramref name="config"/> to enable disk-based content caching
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <param name="directory">
        ///     A path to cache directory
        /// </param>
        /// <param name="enableConcurrentWrites">
        ///     Enable concurrent page cache writes
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsCacheConfigurationBuilder UseDisk(
            [NotNull] this MarkDocsCacheConfigurationBuilder config,
            [NotNull] string directory,
            bool enableConcurrentWrites = true)
        {
            return config.UseDisk(new DiskCacheOptions
            {
                Directory = directory,
                EnableConcurrentWrites = enableConcurrentWrites
            });
        }

        /// <summary>
        ///     Configures <paramref name="config"/> to enable disk-based content caching
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <param name="options">
        ///     Configuration options for disk-based cache
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsCacheConfigurationBuilder UseDisk(
            [NotNull] this MarkDocsCacheConfigurationBuilder config,
            [NotNull] DiskCacheOptions options)
        {
            config.Use(ctx => new DiskCache(ctx.LoggerFactory, options));
            return config;
        }
    }
}