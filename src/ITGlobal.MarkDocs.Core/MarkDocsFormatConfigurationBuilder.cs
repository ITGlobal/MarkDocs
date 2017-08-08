using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs page format configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsFormatConfigurationBuilder
    {
        private readonly IServiceCollection _services;

        internal MarkDocsFormatConfigurationBuilder(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsFormatConfigurationBuilder Use([NotNull] Action<IServiceCollection> func)
        {
            func(_services);
            return this;
        }
    }
}