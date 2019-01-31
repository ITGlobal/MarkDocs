using JetBrains.Annotations;
using System;
using ITGlobal.MarkDocs.Format.Impl;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsOptions"/> to configure page format
    /// </summary>
    [PublicAPI]
    public static class MarkDocsFormatOptionsExtensions
    {
        /// <summary>
        ///     Configures <paramref name="config"/> to use Markdown ad documentation format
        /// </summary>
        [PublicAPI, NotNull]
        public static MarkDocsFormatOptions UseMarkdown(
            [NotNull] this MarkDocsFormatOptions config,
            [CanBeNull] MarkdownOptions options = null)
        {
            options = options ?? new MarkdownOptions();

            config.ConfigureServices(services => options.RegisterServices(services));
            config.Use<MarkdownFormat>();
            return config;
        }

        /// <summary>
        ///     Configures <paramref name="config"/> to use Markdown ad documentation format
        /// </summary>
        [PublicAPI, NotNull]
        public static MarkDocsFormatOptions UseMarkdown(
            [NotNull] this MarkDocsFormatOptions config,
            [NotNull] Action<MarkdownOptions> configure)
        {
            var options = new MarkdownOptions();
            configure(options);
            return config.UseMarkdown(options);
        }
    }
}
