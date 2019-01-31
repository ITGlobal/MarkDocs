using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     MarkDocs page format configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsFormatOptions
    {
        private readonly IServiceCollection _services;
        private Func<IServiceProvider, IFormat> _factory;

        internal MarkDocsFormatOptions(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [NotNull]
        public MarkDocsFormatOptions ConfigureServices([NotNull] Action<IServiceCollection> action)
        {
            action(_services);
            return this;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [NotNull]
        public MarkDocsFormatOptions Use([NotNull] Func<IServiceProvider, IFormat> factory)
        {
            _factory = factory;
            return this;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [NotNull]
        public MarkDocsFormatOptions Use<T>()
            where T : class, IFormat
        {
            ConfigureServices(_ => _.AddSingleton<T>());
            return Use(services => services.GetRequiredService<T>());
        }

        internal void Configure()
        {
            if (_factory == null)
            {
                throw new Exception($"No source format service has been configured");
            }

            _services.AddSingleton(_factory);
        }
    }
}