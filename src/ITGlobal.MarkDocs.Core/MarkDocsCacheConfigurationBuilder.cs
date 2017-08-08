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
        private readonly IServiceCollection _services;

        internal MarkDocsCacheConfigurationBuilder(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsCacheConfigurationBuilder Use([NotNull] Action<IServiceCollection> func)
        {
            func(_services);
            return this;
        }
    }
}