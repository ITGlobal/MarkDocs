using System;
using ITGlobal.MarkDocs.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs cache configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsConfigurationBuilder
    {
        private IMarkDocsEventCallback _eventCallback = new MarkDocsEventCallback();

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

            _eventCallback = callback;
        }

        internal void Configure(IServiceCollection services)
        {
            Storage.Configure(services);
            Format.Configure(services);
            Cache.Configure(services);
            Extensions.Configure(services);
            services.AddSingleton(_eventCallback);
        }
    }
}