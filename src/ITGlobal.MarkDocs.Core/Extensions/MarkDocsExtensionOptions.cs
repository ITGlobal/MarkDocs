using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs.Impl;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Extensions
{
    /// <summary>
    ///     MarkDocs extension configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsExtensionOptions
    {
        private readonly IServiceCollection _services;
        private readonly List<Func<IServiceProvider, IExtensionFactory>> _factories
            = new List<Func<IServiceProvider, IExtensionFactory>>();

        internal MarkDocsExtensionOptions(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionOptions ConfigureServices([NotNull] Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionOptions Use([NotNull] Func<IServiceProvider, IExtensionFactory> factory)
        {
            _factories.Add(factory);
            return this;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionOptions Use<T>()
            where T : class, IExtensionFactory
        {
            ConfigureServices(_ => _.AddSingleton<T>());
            return Use(services => services.GetRequiredService<T>());
        }

        internal void Configure()
        {
            _services.AddSingleton(CreateExtensionCollection);
        }

        private ExtensionCollection CreateExtensionCollection(IServiceProvider serviceProvider)
        {
            return new ExtensionCollection(CreateExtensionFactories());

            IEnumerable<IExtensionFactory> CreateExtensionFactories()
            {
                foreach (var factory in _factories)
                {
                    yield return factory(serviceProvider);
                }
            }
        }
    }
}