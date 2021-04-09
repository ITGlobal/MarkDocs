using System;
using System.Collections.Generic;
using System.Threading;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class BlogEngineImpl : IBlogEngine
    {

        private readonly IMarkDocService _markDocs;
        private readonly IMarkDocsLog _log;

        private readonly object _stateLock = new object();
        private BlogEngineState _state;
        private bool _isDisposed;

        public BlogEngineImpl(IMarkDocService service, IMarkDocsLog log, EventConnector eventConnector)
        {
            _markDocs = service;
            _log = log;
            eventConnector.Engine = this;

            if (service.Documentations.Count > 0)
            {
                if (service.Documentations.Count > 1)
                {
                    _log.Error($"More than one source found. Choosing \"{service.Documentations[0].Id}\" arbitrarily");
                }

                Update(service.Documentations[0]);
            }
        }

        private BlogEngineState State
        {
            get
            {
                lock (_stateLock)
                {
                    while (_state == null && !_isDisposed)
                    {
                        Monitor.Wait(_stateLock);
                    }

                    if (_isDisposed)
                    {
                        throw new ObjectDisposedException(nameof(BlogEngineImpl));
                    }

                    return _state;
                }
            }
            set
            {
                lock (_stateLock)
                {
                    if (_isDisposed)
                    {
                        throw new ObjectDisposedException(nameof(BlogEngineImpl));
                    }

                    _state = value;
                    Monitor.PulseAll(_stateLock);
                }

                _log.Info($"Blog data updated to version \"{value.SourceInfo.LastChangeId}\"");
            }
        }

        #region IBlogEngine

        /// <summary>
        ///     Blog version
        /// </summary>
        public ISourceInfo SourceInfo => State.SourceInfo;

        /// <summary>
        ///     Internal state version
        /// </summary>
        public long StateVersion => State.StateVersion;

        /// <summary>
        ///     Provides errors and warning for blog
        /// </summary>
        public ICompilationReport CompilationReport => State.CompilationReport;

        /// <summary>
        ///     Blog post index
        /// </summary>
        public IBlogIndex Index => State.Index;

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

        /// <summary>
        ///     Updates blog contents
        /// </summary>
        public void Synchronize()
        {
            _markDocs.Synchronize();
        }

        #endregion

        #region IDisposable

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            lock (_stateLock)
            {
                _isDisposed = true;
                Monitor.PulseAll(_stateLock);
            }

            _markDocs.Dispose();
        }

        #endregion

        #region internal methods

        internal void Update(IMarkDocState state)
        {
            var report = new BlogCompilationReportBuilder();
            if (state.List.Length > 1)
            {
                report.Error($"More than one source found. Choosing \"{state.List[0].Id}\" arbitrarily");
                _log.Error($"More than one source found. Choosing \"{state.List[0].Id}\" arbitrarily");
            }

            State = new BlogEngineState(this, state.List[0], report);
        }

        internal void Update(IDocumentation documentation)
        {
            var report = new BlogCompilationReportBuilder();
            State = new BlogEngineState(this, documentation, report);
        }

        #endregion

    }
}