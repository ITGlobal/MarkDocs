using ITGlobal.MarkDocs.Cache.Impl;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsOptions"/> to configure cache
    /// </summary>
    [PublicAPI]
    public static class MarkDocsOptionsExtensions
    {
        /// <summary>
        ///     Configures <paramref name="options"/> to enable disk-based content caching
        /// </summary>
        /// <param name="options">
        ///     Configuration builder
        /// </param>
        /// <param name="directory">
        ///     A path to cache directory
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsOptions UseDiskCache(
            [NotNull] this MarkDocsOptions options,
            [NotNull] string directory)
        {
            options.ConfigureServices(_ => _.AddSingleton(new DiskCacheOptions { Directory = directory }));
            options.ConfigureServices(_ => _.AddSingleton<DiskCacheProvider>());
            return options.UseCache(_ => _.GetRequiredService<DiskCacheProvider>());
        }
    }
}