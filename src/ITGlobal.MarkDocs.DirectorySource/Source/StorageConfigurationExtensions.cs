using ITGlobal.MarkDocs.Source.Impl;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsSourceTreeOptions"/> to configure storage
    /// </summary>
    [PublicAPI]
    public static class StorageConfigurationExtensions
    {
        /// <summary>
        ///      Configures <paramref name="config"/> to use static directory as documentation source tree
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <param name="directory">
        ///     Path to source directory
        /// </param>
        /// <param name="watchForChanges">
        ///     Watch for source changes
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsSourceTreeOptions UseStaticDirectory(
            [NotNull] this MarkDocsSourceTreeOptions config,
            [NotNull] string directory,
            bool watchForChanges = false)
        {
            return config.UseStaticDirectory(new[] { directory }, watchForChanges);
        }

        /// <summary>
        ///      Configures <paramref name="config"/> to use static directory as documentation source tree
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <param name="directories">
        ///     Pathes to source directories
        /// </param>
        /// <param name="watchForChanges">
        ///     Watch for source changes
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsSourceTreeOptions UseStaticDirectory(
            [NotNull] this MarkDocsSourceTreeOptions config,
            [NotNull] string[] directories,
            bool watchForChanges = false)
        {
            return config.UseStaticDirectory(new StaticDirectorySourceTreeOptions
            {
                Directories = directories,
                WatchForChanges = watchForChanges
            });
        }

        /// <summary>
        ///      Configures <paramref name="config"/> to use static directory as documentation source tree
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <param name="options">
        ///     Options for static directory source tree
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsSourceTreeOptions UseStaticDirectory(
            [NotNull] this MarkDocsSourceTreeOptions config,
            [NotNull] StaticDirectorySourceTreeOptions options)
        {
            config.ConfigureServices(_ => _.AddSingleton(options));
            config.Use<StaticDirectorySourceTreeProvider>();
            return config;
        }
    }
}
