using ITGlobal.MarkDocs.Impl;
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
        /// <summary>
        ///     Sets a lifetime event callback
        /// </summary>
        [NotNull]
        public static MarkDocsOptions UseCallback([NotNull] this MarkDocsOptions options, [NotNull] IMarkDocsEventCallback callback)
        {
            return options.UseCallback(_ => callback);
        }

        /// <summary>
        ///     Sets a lifetime event callback
        /// </summary>
        [NotNull]
        public static MarkDocsOptions UseCallback<T>([NotNull] this MarkDocsOptions options)
            where T: class, IMarkDocsEventCallback
        {
            options.ConfigureServices(_ => _.AddSingleton<T>());
            return options.UseCallback(_ => _.GetRequiredService<T>());
        }

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
    }
}