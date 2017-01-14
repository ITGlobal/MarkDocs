using System;
using System.Linq;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
        public static void AddMarkDocs(this IServiceCollection services, Action<MarkDocsConfigurationBuilder> configure)
        {
            var builder = new MarkDocsConfigurationBuilder();

            builder.Cache.UseNull();

            configure(builder);
            builder.Configure(services);

            services.AddSingleton<MarkDocService>();
            services.AddSingleton<IMarkDocService>(CreateService);

            VerifyService<IStorage>(services);
            VerifyService<IFormat>(services);
            VerifyService<ICache>(services);
        }

        private static IMarkDocService CreateService(IServiceProvider sp)
        {
            IMarkDocService service = sp.GetRequiredService<MarkDocService>();
            var lifetime = sp.GetService<IApplicationLifetime>();

            if (lifetime != null)
            {
                lifetime.ApplicationStopping.Register(() => service.Dispose());
            }

            return service;
        }

        private static void VerifyService<T>(IServiceCollection services)
        {
            var service = services.FirstOrDefault(_ => _.ServiceType == typeof(T));
            if (service == null)
            {
                throw new Exception($"Service {typeof(T).FullName} is not configured");
            }
        }
    }
}