using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Static factory to create standalone instances of <see cref="IMarkDocService"/>
    /// </summary>
    [PublicAPI]
    public static class MarkDocsFactory
    {
        private sealed class StandaloneMarkDocService : IMarkDocService
        {
            private readonly ServiceProvider _serviceProvider;
            private readonly IMarkDocService _service;

            public StandaloneMarkDocService(ServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
                _service = serviceProvider.GetRequiredService<IMarkDocService>();
            }

            IReadOnlyList<IDocumentation> IMarkDocService.Documentations 
                => _service.Documentations;

            IDocumentation IMarkDocService.GetDocumentation(string documentationId)
                => _service.GetDocumentation(documentationId);

            void IMarkDocService.Synchronize() 
                => _service.Synchronize();

            TExtension IMarkDocService.GetExtension<TExtension>() 
                => _service.GetExtension<TExtension>();

            void IDisposable.Dispose() 
                => _serviceProvider.Dispose();
        }

        /// <summary>
        ///     Creates a standalone instance of <see cref="IMarkDocService"/>
        /// </summary>
        [NotNull]
        public static IMarkDocService Create([NotNull] Action<MarkDocsOptions> configure)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMarkDocs(configure);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            return new StandaloneMarkDocService(serviceProvider);
        }
    }
}