using System;
using ITGlobal.MarkDocs.Cache;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Static factory to create standalone instances of <see cref="IMarkDocService"/>
    /// </summary>
    public static class MarkDocsFactory
    {
        /// <summary>
        ///     Creates a standalone instance of <see cref="IMarkDocService"/>
        /// </summary>
        [PublicAPI, NotNull]
        public static IMarkDocService Create(
            [NotNull] Action<MarkDocsConfigurationBuilder> configure,
            ILoggerFactory loggerFactory = null
        )
        {
            var builder = new MarkDocsConfigurationBuilder();
            builder.Cache.UseNull();
            configure(builder);

            var markdocs = builder.BuildInstance(loggerFactory ?? new LoggerFactory());
            return markdocs;
        }
    }
}