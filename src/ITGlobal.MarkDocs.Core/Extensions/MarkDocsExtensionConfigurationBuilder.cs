using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Extensions
{
    /// <summary>
    ///     MarkDocs extension configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsExtensionConfigurationBuilder
    {
        private readonly IServiceCollection _services;

        internal MarkDocsExtensionConfigurationBuilder(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        ///     Adds an extension
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionConfigurationBuilder Add([NotNull] Func<IServiceCollection, Func<IServiceProvider, IExtensionFactory>> factory)
        {
            _services.AddSingleton(factory(_services));
            return this;
        }

        /// <summary>
        ///     Adds an extension
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionConfigurationBuilder Add<TExtensionFactory>(Action<IServiceCollection> configureServices = null)
            where TExtensionFactory : IExtensionFactory
        {
            if (configureServices != null)
            {
                configureServices(_services);
            }

            _services.AddSingleton(typeof(IExtensionFactory), typeof(TExtensionFactory));
            return this;
        }
    }
}