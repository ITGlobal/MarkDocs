using System;
using System.Collections.Generic;
using System.Threading;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Impl;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class BlogEngineImpl : IBlogEngine
    {
        private readonly IMarkDocService _markDocs;
        private readonly IMarkDocsLog _log;

        private BlogEngineState _state;

        public BlogEngineImpl(IMarkDocService service, IMarkDocsLog log, EventConnector eventConnector)
        {
            _markDocs = service;
            _log = log;
            eventConnector.Engine = this;

            Update(service.Documentations[0]);
        }

        private BlogEngineState State
        {
            get
            {
                return Interlocked.CompareExchange(ref _state, null, null);
            }
            set
            {
                Interlocked.Exchange(ref _state, value);

                _log.Info($"Blog data updated to version \"{value.SourceInfo.LastChangeId}\"");
            }
        }

        #region IBlogEngine
        
        /// <summary>
        ///     Blog version
        /// </summary>
        public ISourceInfo SourceInfo => State.SourceInfo;

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
