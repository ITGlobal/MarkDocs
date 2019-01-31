using System;
using ITGlobal.MarkDocs.Impl;
using ITGlobal.MarkDocs.Source;
using ITGlobal.MarkDocs.Source.Impl;
using JetBrains.Annotations;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///    Extension methods for setting up MarkDocs services in an <see cref="IServiceCollection" />.
    /// </summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds MarkDocs services into <paramref name="services"/>
        /// </summary>
        /// <param name="services">
        ///     Service collection to configure
        /// </param>
        /// <param name="configure">
        ///     MarkDocs configuration function
        /// </param>
        [PublicAPI]
        public static void AddMarkDocs(this IServiceCollection services, Action<MarkDocsOptions> configure)
        {
            var builder = new MarkDocsOptions(services);

            configure(builder);
            builder.Configure();

            services.AddSingleton<IMarkDocService, MarkDocService>();
            services.AddSingleton<IAssetTreeReader, AssetTreeReader>();
            services.TryAddSingleton<IContentHashProvider, Sha1ContentHashProvider>();
            services.TryAddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
            services.AddSingleton<IContentMetadataProvider, DefaultContentMetadataProvider>();
        }
    }
}