using System;
using ITGlobal.MarkDocs.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs cache configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsConfigurationBuilder
    {
        private IMarkDocsEventCallback _callback = new MarkDocsEventCallback();

        /// <summary>
        ///     Storage configuration builder
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsStorageConfigurationBuilder Storage { get; } = new MarkDocsStorageConfigurationBuilder();

        /// <summary>
        ///     Page format configuration builder
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsFormatConfigurationBuilder Format { get; } = new MarkDocsFormatConfigurationBuilder();

        /// <summary>
        ///     Content cache configuration builder
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsCacheConfigurationBuilder Cache { get; } = new MarkDocsCacheConfigurationBuilder();

        /// <summary>
        ///     Extensions configuration builder
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionConfigurationBuilder Extensions { get; } = new MarkDocsExtensionConfigurationBuilder();

        /// <summary>
        ///     Sets a lifetime event callback
        /// </summary>
        [PublicAPI]
        public void UseCallback([NotNull] IMarkDocsEventCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _callback = callback;
        }

        internal Func<IServiceProvider, IMarkDocService> Build()
        {
            return services =>
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                return BuildInstance(loggerFactory);
            };
        }

        internal IMarkDocService BuildInstance(ILoggerFactory loggerFactory)
        {
            var context = new MarkdocsFactoryContext(loggerFactory, _callback);

            var storage = Storage.Build(context);
            var format = Format.Build(context);
            var cache = Cache.Build(context);
            var extensionFactories = Extensions.Build(context);

            var markdocs = new MarkDocService(
                loggerFactory,
                storage,
                format,
                cache,
                _callback,
                extensionFactories
                );
            return markdocs;
        }
    }
}