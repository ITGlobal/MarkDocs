﻿using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsConfigurationBuilder"/> to configure page format
    /// </summary>
    [PublicAPI]
    public static class FormatConfigurationExtensions
    {
        /// <summary>
        ///     Configures <paramref name="config"/> to use Markdown ad documentation format
        /// </summary>
        [PublicAPI, NotNull]
        public static MarkDocsFormatConfigurationBuilder UseMarkdown(
            [NotNull] this MarkDocsFormatConfigurationBuilder config,
            [CanBeNull] MarkdownOptions options = null,
            [CanBeNull] Func<IServiceProvider, IResourceUrlResolver> resourceUrlResolverFactory = null

            )
        {
            options = options ?? new MarkdownOptions();
            resourceUrlResolverFactory = resourceUrlResolverFactory ?? (_ => new DefaultResourceUrlResolver());

            config.Use(services =>
            {
                services.AddSingleton(options);
                services.AddSingleton(resourceUrlResolverFactory);
                services.AddSingleton<IFormat, MarkdownFormat>();
            });
            return config;
        }
    }
}
