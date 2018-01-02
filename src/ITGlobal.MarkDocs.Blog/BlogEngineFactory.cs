using System;
using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Static factory to create standalone instances of <see cref="IBlogEngine"/>
    /// </summary>
    public static class BlogEngineFactory
    {
        /// <summary>
        ///     Creates a standalone instance of <see cref="IBlogEngine"/>
        /// </summary>
        [PublicAPI, NotNull]
        public static IBlogEngine Create(
            [NotNull] string workingDirectory,
            [NotNull] IResourceUrlResolver urlResolver,
            [NotNull] Action<BlogEngineConfigurationBuilder> configure,
            ILoggerFactory loggerFactory = null
        )
        {
            var builder = new BlogEngineConfigurationBuilder(workingDirectory, urlResolver);
            configure(builder);

            var engine = builder.BuildInstance(loggerFactory ?? new LoggerFactory());
            return engine;
        }
    }
}