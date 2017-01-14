using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs content cache configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsCacheConfigurationBuilder
    {
        private Action<IServiceCollection> _registration;

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsCacheConfigurationBuilder Use([NotNull] Action<IServiceCollection> func)
        {
            _registration = func;
            return this;
        }

        internal void Configure(IServiceCollection services)
        {
            _registration?.Invoke(services);
        }
    }
}