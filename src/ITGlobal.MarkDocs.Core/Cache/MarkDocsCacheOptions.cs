using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     MarkDocs content cache configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsCacheOptions
    {
        private readonly IServiceCollection _services;
        private Func<IServiceProvider, ICacheProvider> _factory;

        internal MarkDocsCacheOptions(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsCacheOptions ConfigureServices([NotNull] Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }
        
        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsCacheOptions Use([NotNull] Func<IServiceProvider, ICacheProvider> factory)
        {
            _factory = factory;
            return this;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsCacheOptions Use<T>()
            where T : class, ICacheProvider
        {
            ConfigureServices(_ => _.AddSingleton<T>());
            return Use(services => services.GetRequiredService<T>());
        }

        internal void Configure()
        {
            if (_factory == null)
            {
                throw new Exception($"No cache service has been configured");
            }

            _services.AddSingleton(_factory);
        }
    }
}