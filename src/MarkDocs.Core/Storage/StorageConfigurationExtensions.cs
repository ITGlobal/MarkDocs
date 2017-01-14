using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Storage
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsStorageConfigurationBuilder"/> to configure storage
    /// </summary>
    [PublicAPI]
    public static class StorageConfigurationExtensions
    {
        /// <summary>
        ///      Configures <paramref name="config"/> to use static directory as documentation source
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <param name="contentDirectory">
        ///     Path to source directory
        /// </param>
        /// <param name="enableWatch">
        ///     Watch for source changes
        /// </param>
        /// <param name="useSubdirectories">
        ///     Map each subdirectory to a separate documentation
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsStorageConfigurationBuilder UseStaticDirectory(
            [NotNull] this MarkDocsStorageConfigurationBuilder config,
            [NotNull] string contentDirectory,
            bool enableWatch = false,
            bool useSubdirectories = false)
        {
            return config.UseStaticDirectory(new StaticStorageOptions
            {
                Directory = contentDirectory,
                EnableWatch = enableWatch,
                UseSubdirectories = useSubdirectories
            });
        }

        /// <summary>
        ///      Configures <paramref name="config"/> to use static directory as documentation source
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <param name="options">
        ///     Options for static directory storage
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsStorageConfigurationBuilder UseStaticDirectory(
            [NotNull] this MarkDocsStorageConfigurationBuilder config,
            [NotNull] StaticStorageOptions options)
        {
            config.Use(services =>
            {
                services.AddSingleton(options);
                if (options.UseSubdirectories)
                {
                    services.AddSingleton<IStorage, StaticMultiStorage>();
                }
                else
                {
                    services.AddSingleton<IStorage, StaticStorage>();
                }
            });
            return config;
        }
    }
}