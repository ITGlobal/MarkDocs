using System;
using JetBrains.Annotations;
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
        public static void AddMarkDocs(this IServiceCollection services, Action<MarkDocsOptions> configure)
        {
            var builder = new MarkDocsOptions(services);

            configure(builder);
            builder.Configure();
        }
    }
}