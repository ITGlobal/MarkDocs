using ITGlobal.MarkDocs.Format.Impl;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsOptions"/> to configure page format
    /// </summary>
    [PublicAPI]
    public static class MarkDocsOptionsExtensions
    {
        /// <summary>
        ///     Configures <paramref name="config"/> to use Markdown ad documentation format
        /// </summary>
        [PublicAPI, NotNull]
        public static MarkDocsOptions UseMarkdown(
            [NotNull] this MarkDocsOptions config,
            [CanBeNull] MarkdownOptions options = null)
        {
            options = options ?? new MarkdownOptions();

            config.ConfigureServices(services => options.RegisterServices(services));
            config.ConfigureServices(services => services.AddSingleton<MarkdownFormat>());
            config.UseFormat(_ => _.GetRequiredService<MarkdownFormat>());
            return config;
        }

        /// <summary>
        ///     Configures <paramref name="config"/> to use Markdown ad documentation format
        /// </summary>
        [PublicAPI, NotNull]
        public static MarkDocsOptions UseMarkdown(
            [NotNull] this MarkDocsOptions config,
            [NotNull] Action<MarkdownOptions> configure)
        {
            var options = new MarkdownOptions();
            configure(options);
            return config.UseMarkdown(options);
        }
    }
}
