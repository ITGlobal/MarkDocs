using ITGlobal.MarkDocs.Impl;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Extension methods for <see cref="BlogEngineOptions"/>
    /// </summary>
    [PublicAPI]
    public static class BlogEngineOptionsExtensions
    {
        #region ResourceUrlResolver

        /// <summary>
        ///     Sets resource URL resolver implementation
        /// </summary>
        [NotNull]
        public static BlogEngineOptions UseResourceUrlResolver([NotNull] this BlogEngineOptions options, [NotNull] IResourceUrlResolver resolver)
            => options.UseResourceUrlResolver(_ => resolver);

        /// <summary>
        ///     Sets resource URL resolver implementation
        /// </summary>
        [NotNull]
        public static BlogEngineOptions UseResourceUrlResolver<T>([NotNull] this BlogEngineOptions options)
            where T : class, IResourceUrlResolver
        {
            options.ConfigureServices(_ => _.AddSingleton<T>());
            return options.UseResourceUrlResolver(_ => _.GetRequiredService<T>());
        }

        #endregion

        #region Logger

        /// <summary>
        ///     Sets a logger implementation
        /// </summary>
        [NotNull]
        public static BlogEngineOptions UseLog([NotNull] this BlogEngineOptions options, [NotNull] IMarkDocsLog log)
        {
            return options.UseLog(_ => log);
        }

        /// <summary>
        ///     Sets a logger implementation
        /// </summary>
        [NotNull]
        public static BlogEngineOptions UseLog<T>([NotNull] this BlogEngineOptions options)
            where T : class, IMarkDocsLog
        {
            options.ConfigureServices(_ => _.AddSingleton<T>());
            return options.UseLog(_ => _.GetRequiredService<T>());
        }

        /// <summary>
        ///     Sets a logger implementation
        /// </summary>
        [NotNull]
        public static BlogEngineOptions UseAspNetLog([NotNull] this BlogEngineOptions options)
        {
            return options.UseLog<AspNetMarkDocsLog>();
        }

        #endregion
    }
}