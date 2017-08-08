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
        private readonly IServiceCollection _services;

        internal MarkDocsConfigurationBuilder(IServiceCollection services)
        {
            _services = services;

            Storage = new MarkDocsStorageConfigurationBuilder(services);
            Format = new MarkDocsFormatConfigurationBuilder(services);
            Cache = new MarkDocsCacheConfigurationBuilder(services);
            Extensions = new MarkDocsExtensionConfigurationBuilder(services);

            _services.AddSingleton<IMarkDocsEventCallback>(new MarkDocsEventCallback());
        }

        /// <summary>
        ///     Storage configuration builder
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsStorageConfigurationBuilder Storage { get; }

        /// <summary>
        ///     Page format configuration builder
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsFormatConfigurationBuilder Format { get; }

        /// <summary>
        ///     Content cache configuration builder
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsCacheConfigurationBuilder Cache { get; }

        /// <summary>
        ///     Extensions configuration builder
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionConfigurationBuilder Extensions { get; }

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

            _services.AddSingleton(callback);
        }
    }
}