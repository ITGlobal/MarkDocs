using ITGlobal.MarkDocs.Source.Impl;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsOptions"/> to configure storage
    /// </summary>
    [PublicAPI]
    public static class MarkDocsSourceTreeOptionsExtensions
    {
        /// <summary>
        ///      Configures <paramref name="config"/> to use git as documentation source
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
        public static MarkDocsOptions FromGit(
            [NotNull] this MarkDocsOptions config,
            [NotNull] GitSourceTreeOptions options)
        {
            config.ConfigureServices(_ => _.AddSingleton(options));
            config.ConfigureServices(_ => _.AddSingleton<GitSourceTreeProvider>());
            config.UseSourceTree(_ => _.GetRequiredService<GitSourceTreeProvider>());
            return config;
        }

        /// <summary>
        ///      Configures <paramref name="config"/> to use git as documentation source
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
        public static MarkDocsOptions FromGit(
            [NotNull] this MarkDocsOptions config,
            [NotNull] Action<GitSourceTreeOptions> setup)
        {
            var options = new GitSourceTreeOptions();
            setup(options);

            return config.FromGit(options);
        }
    }
}