using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Static factory to create standalone instances of <see cref="IBlogEngine"/>
    /// </summary>
    public static class BlogEngineFactory
    {
        private sealed class StandaloneBlogEngine : IBlogEngine
        {
            private readonly ServiceProvider _serviceProvider;
            private readonly IBlogEngine _engine;

            public StandaloneBlogEngine(ServiceProvider serviceProvider, IBlogEngine engine)
            {
                _serviceProvider = serviceProvider;
                _engine = engine;
            }

            public ISourceInfo SourceInfo => _engine.SourceInfo;
            public ICompilationReport CompilationReport => _engine.CompilationReport;
            public IBlogIndex Index => _engine.Index;

            public IBlogResource GetResource(string id) => _engine.GetResource(id);
            public ITextSearchResult Search(string query) => _engine.Search(query);
            public IReadOnlyList<string> Suggest(string query) => _engine.Suggest(query);
            public void Synchronize() => _engine.Synchronize();

            public void Dispose()
            {
                _engine.Dispose();
                _serviceProvider.Dispose();
            }
        }

        /// <summary>
        ///     Creates a standalone instance of <see cref="IBlogEngine"/>
        /// </summary>
        [PublicAPI, NotNull]
        public static IBlogEngine Create(
            [NotNull] string workingDirectory,
            [NotNull] Action<BlogEngineOptions> configure,
            ILoggerFactory loggerFactory = null
        )
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddMarkdocsBlogEngine(workingDirectory, configure);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var engine = serviceProvider.GetRequiredService<IBlogEngine>();
            
            return new StandaloneBlogEngine(serviceProvider, engine);
        }
    }
}