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
        /// <summary>
        ///     Storage configuration builder
        /// </summary>
        [NotNull]
        public MarkDocsStorageConfigurationBuilder Storage { get; } = new MarkDocsStorageConfigurationBuilder();

        /// <summary>
        ///     Page format configuration builder
        /// </summary>
        [NotNull]
        public MarkDocsFormatConfigurationBuilder Format { get; } = new MarkDocsFormatConfigurationBuilder();

        /// <summary>
        ///     Content cache configuration builder
        /// </summary>
        [NotNull]
        public MarkDocsCacheConfigurationBuilder Cache { get; } = new MarkDocsCacheConfigurationBuilder();

        /// <summary>
        ///     Extensions configuration builder
        /// </summary>
        [NotNull]
        public MarkDocsExtensionConfigurationBuilder Extensions { get; } = new MarkDocsExtensionConfigurationBuilder();
        
        internal void Configure(IServiceCollection services)
        {
            Storage.Configure(services);
            Format.Configure(services);
            Cache.Configure(services);
            Extensions.Configure(services);
        }
    }
}