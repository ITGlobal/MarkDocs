using System;
using System.Collections.Generic;
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
        private List<Action<IServiceCollection>> _registrations = new List<Action<IServiceCollection>>();
        
        /// <summary>
        ///     Adds an extension
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionConfigurationBuilder Add([NotNull] Func<IServiceProvider, IExtensionFactory> factory)
        {
            _registrations.Add(sp => sp.AddSingleton(factory));
            return this;
        }
        
        /// <summary>
        ///     Adds an extension
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionConfigurationBuilder Add<TExtensionFactory>()
            where TExtensionFactory : IExtensionFactory
        {
            _registrations.Add(sp => sp.AddSingleton(typeof(IExtensionFactory), typeof(TExtensionFactory)));
            return this;
        }

        internal void Configure(IServiceCollection services)
        {
            foreach (var action in _registrations)
            {
                action(services);
            }
        }
    }
}