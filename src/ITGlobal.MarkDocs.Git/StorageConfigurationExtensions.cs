using System;
using ITGlobal.MarkDocs;
using ITGlobal.MarkDocs.Storage;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Git
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsStorageConfigurationBuilder"/> to configure storage
    /// </summary>
    [PublicAPI]
    public static class StorageConfigurationExtensions
    {
        /// <summary>
        ///      Configures <paramref name="config"/> to use gitas documentation source
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <param name="options">
        ///     Options for git storage
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsStorageConfigurationBuilder UseGit(
            [NotNull] this MarkDocsStorageConfigurationBuilder config,
            [NotNull] GitStorageOptions options)
        {
            config.Use(services =>
            {
                services.AddSingleton(options);
                services.AddSingleton<IStorage, GitStorage>();
            });
            return config;
        }

        /// <summary>
        ///      Configures <paramref name="config"/> to use gitas documentation source
        /// </summary>
        /// <param name="config">
        ///     Configuration builder
        /// </param>
        /// <param name="setup">
        ///     Configuration function
        /// </param>
        /// <returns>
        ///     Configuration builder (fluent interface)
        /// </returns>
        [PublicAPI, NotNull]
        public static MarkDocsStorageConfigurationBuilder UseGit(
            [NotNull] this MarkDocsStorageConfigurationBuilder config,
            [NotNull] Action<GitStorageOptions> setup)
        {
            var options = new GitStorageOptions();
            setup(options);

            return config.UseGit(options);
        }
    }
}