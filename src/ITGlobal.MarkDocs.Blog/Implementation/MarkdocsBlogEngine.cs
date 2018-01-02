using System;
using System.Collections.Generic;
using System.Threading;
using ITGlobal.MarkDocs.Extensions;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Blog.Implementation
{
    internal sealed class MarkdocsBlogEngine : IBlogEngine
    {
        private readonly IMarkDocService _service;
        private readonly ILogger _logger;
        
        private readonly ManualResetEventSlim _initialized = new ManualResetEventSlim();

        private BlogEngineState _state;

        public MarkdocsBlogEngine(IMarkDocService service, ILoggerFactory loggerFactory)
        {
            _service = service;
            _logger = loggerFactory.CreateLogger(typeof(MarkdocsBlogEngine));
        }

        private BlogEngineState State
        {
            get
            {
                _initialized.Wait();
                return Interlocked.CompareExchange(ref _state, null, null);
            }
            set
            {
                Interlocked.Exchange(ref _state, value);
                _initialized.Set();
                
                _logger.LogInformation("Blog data updated to version \"{0}\"", value.ContentVersion.LastChangeId);
            }
        }

        #region IBlogEngine

        /// <summary>
        ///     Blog version
        /// </summary>
        public IContentVersion ContentVersion => State.ContentVersion;

        /// <summary>
        ///     Provides errors and warning for blog
        /// </summary>
        public ICompilationReport CompilationReport => State.CompilationReport;

        /// <summary>
        ///     Blog post index
        /// </summary>
        public IBlogIndex Index => State.Index;

        /// <summary>
        ///     Initializes blog data (foreground)
        /// </summary>
        public void Initialize() => _service.Initialize();

        /// <summary>
        ///     Gets a blog resource by its ID
        /// </summary>
        /// <param name="id">
        ///     Blog resource URL or permalink
        /// </param>
        /// <returns>
        ///     A blog resource or null if resource doesn't exist
        /// </returns>
        public IBlogResource GetResource(string id) => State.GetResource(id);

        /// <summary>
        ///     Search blog posts by text
        /// </summary>
        public ITextSearchResult Search(string query) => State.Search(query);
        
        /// <summary>
        ///     Gets a list of search query suggestions
        /// </summary>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     List of search suggestions
        /// </returns>
        public IReadOnlyList<string> Suggest(string query) => State.Suggest(query);

        #endregion

        #region IDisposable

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            _initialized.Dispose();
            _service.Dispose();
        }

        #endregion

        #region internal methods

        internal void Update(IMarkDocServiceState state)
        {
            State = new BlogEngineState(this, state);
        }

        #endregion
    }
}
