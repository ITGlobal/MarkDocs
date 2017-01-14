using System.Collections.Concurrent;
using ITGlobal.MarkDocs;
using ITGlobal.MarkDocs.Comments.Data;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Format;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     An extension that provides access to page comments
    /// </summary>
    internal sealed class CommentsExtension : IExtension
    {
        private readonly ICommentDataRepository _repository;
        private readonly IFormat _format;
        private readonly ILogger _log;

        private readonly ConcurrentDictionary<string, IPageComments> _pageComments
            = new ConcurrentDictionary<string, IPageComments>();

        public CommentsExtension(ICommentDataRepository repository, IFormat format, ILoggerFactory loggerFactory, IMarkDocServiceState state)
        {
            _repository = repository;
            _format = format;
            _log = loggerFactory.CreateLogger<CommentsExtension>();

            SynchronizeWithState(state);
        }

        /// <summary>
        ///     Gets comments for a page
        /// </summary>
        /// <param name="page">
        ///     Page
        /// </param>
        /// <returns>
        ///     Comments for a page
        /// </returns>
        public IPageComments GetPageComments(IPage page)
            => _pageComments.GetOrAdd(page.Id, _ => new PageComments(_repository, _format, _log, page));

        /// <summary>
        ///     Handle a documentation state update
        /// </summary>
        /// <param name="state">
        ///     New documentation state
        /// </param>
        void IExtension.Update(IMarkDocServiceState state) => SynchronizeWithState(state);

        private void SynchronizeWithState(IMarkDocServiceState state)
        {
            foreach (var documentation in state.List)
            {
                var pageIds = _repository.LoadPageIds(documentation.Id);

                foreach (var id in pageIds)
                {
                    if (documentation.GetPage(id) == null)
                    {
                        if (_repository.DeletePageComments(documentation.Id, id))
                        {
                            _log.LogDebug("Comments for non-existing page '{0}:{1}' were dropped", documentation.Id, id);
                        }
                    }
                }
            }
        }
    }
}