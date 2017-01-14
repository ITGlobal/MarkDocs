using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs storage configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsStorageConfigurationBuilder
    {
        private Action<IServiceCollection> _registration;

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsStorageConfigurationBuilder Use([NotNull] Action<IServiceCollection> func)
        {
            _registration = func;
            return this;
        }

        internal void Configure(IServiceCollection services) => _registration?.Invoke(services);
    }
}