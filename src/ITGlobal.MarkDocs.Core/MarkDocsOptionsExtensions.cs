using ITGlobal.MarkDocs.Impl;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsOptions"/>
    /// </summary>
    [PublicAPI]
    public static class MarkDocsOptionsExtensions
    {
        #region ResourceUrlResolver
        
        /// <summary>
        ///     Sets resource URL resolver implementation
        /// </summary>
        [NotNull]
        public static MarkDocsOptions UseResourceUrlResolver([NotNull] this MarkDocsOptions options, [NotNull] IResourceUrlResolver resolver)
            => options.UseResourceUrlResolver(_ => resolver);

        /// <summary>
        ///     Sets resource URL resolver implementation
        /// </summary>
        [NotNull]
        public static MarkDocsOptions UseResourceUrlResolver<T>([NotNull] this MarkDocsOptions options)
            where T : class, IResourceUrlResolver
        {
            options.ConfigureServices(_ => _.AddSingleton<T>());
            return options.UseResourceUrlResolver(_ => _.GetRequiredService<T>());
        }

        #endregion

        #region Callback

        /// <summary>
        ///     Sets a lifetime event listener
        /// </summary>
        [NotNull]
        public static MarkDocsOptions UseEventListener([NotNull] this MarkDocsOptions options, [NotNull] MarkDocsEventListener listener)
        {
            return options.UseCallback(_ => listener);
        }

        /// <summary>
        ///     Sets a lifetime event listener
        /// </summary>
        [NotNull]
        public static MarkDocsOptions UseEventListener<T>([NotNull] this MarkDocsOptions options)
            where T: MarkDocsEventListener
        {
            options.ConfigureServices(_ => _.AddSingleton<T>());
            return options.UseCallback(_ => _.GetRequiredService<T>());
        }

        #endregion

        #region Logger

        /// <summary>
        ///     Sets a logger implementation
        /// </summary>
        [NotNull]
        public static MarkDocsOptions UseLog([NotNull] this MarkDocsOptions options, [NotNull] IMarkDocsLog log)
        {
            return options.UseLog(_ => log);
        }

        /// <summary>
        ///     Sets a logger implementation
        /// </summary>
        [NotNull]
        public static MarkDocsOptions UseLog<T>([NotNull] this MarkDocsOptions options)
            where T: class, IMarkDocsLog
        {
            options.ConfigureServices(_ => _.AddSingleton<T>());
            return options.UseLog(_ => _.GetRequiredService<T>());
        }

        /// <summary>
        ///     Sets a logger implementation
        /// </summary>
        [NotNull]
        public static MarkDocsOptions UseAspNetLog([NotNull] this MarkDocsOptions options)
        {
            return options.UseLog<AspNetMarkDocsLog>();
        }

        #endregion
    }
}