using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs source tree configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsSourceTreeOptions
    {
        private readonly IServiceCollection _services;
        private Func<IServiceProvider, ISourceTreeProvider> _factory;

        internal MarkDocsSourceTreeOptions(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [NotNull]
        public MarkDocsSourceTreeOptions ConfigureServices([NotNull] Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [NotNull]
        public MarkDocsSourceTreeOptions Use([NotNull] Func<IServiceProvider, ISourceTreeProvider> factory)
        {
            _factory = factory;
            return this;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [NotNull]
        public MarkDocsSourceTreeOptions Use<T>()
            where T : class, ISourceTreeProvider
        {
            ConfigureServices(_ => _.AddSingleton<T>());
            return Use(services => services.GetRequiredService<T>());
        }

        internal void Configure()
        {
            if (_factory == null)
            {
                throw new Exception($"No source tree provides has been configured");
            }

            _services.AddSingleton(_factory);
        }
    }
}