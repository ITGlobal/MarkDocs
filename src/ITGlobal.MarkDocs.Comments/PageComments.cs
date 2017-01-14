using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs;
using ITGlobal.MarkDocs.Comments.Data;
using ITGlobal.MarkDocs.Format;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     Comments for a page
    /// </summary>
    internal sealed class PageComments : IPageComments, ICommentParent
    {
        #region fields

        private readonly ICommentDataRepository _repository;
        private readonly IFormat _format;
        private readonly ILogger _log;
        private readonly IPage _page;
        
        private readonly object _commentsLock = new object();
        private List<IComment> _comments;
        private Dictionary<string, IComment> _commentsById;

        #endregion

        #region .ctor

        public PageComments(ICommentDataRepository repository, IFormat format, ILogger log, IPage page)
        {
            _repository = repository;
            _format = format;
            _log = log;
            _page = page;
        }

        #endregion

        #region IPageComments

        /// <summary>
        ///     List of comments
        /// </summary>
        public IReadOnlyList<IComment> Comments => GetOrLoadComments();

        /// <summary>
        ///     Adds new comment
        /// </summary>
        /// <param name="user">
        ///     User ID
        /// </param>
        /// <param name="markup">
        ///     Comment markup
        /// </param>
        /// <returns>
        ///     New comment
        /// </returns>
        public IComment AddComment(string user, string markup)
        {
            lock (_commentsLock)
            {
                var comments = GetOrLoadComments();
                var data = _repository.CreateComment(_page.Documentation.Id, _page.Id, user, markup);

                var comment = new Comment(this, data);
                comments.Add(comment);
                return comment;
            }
        }

        /// <summary>
        ///     Find a comment or reply by its ID
        /// </summary>
        /// <param name="id">
        ///     Comment ID
        /// </param>
        /// <returns>
        ///     A comment or null
        /// </returns>
        public IComment FindComment(string id)
        {
            lock (_commentsLock)
            {
                IComment comment = null;
                _commentsById?.TryGetValue(id, out comment);
                return comment;
            }
        }

        #endregion

        #region ICommentParent

        IPage ICommentParent.Page => _page;
        ICommentDataRepository ICommentParent.Repository => _repository;
        IFormat ICommentParent.Format => _format;

        void ICommentParent.OnAdded(IComment comment)
        {
            lock (_commentsLock)
            {
                _comments?.Remove(comment);
                if (_commentsById != null)
                {
                    _commentsById[comment.Id] = comment;
                }
            }
        }

        void ICommentParent.OnDeleted(IComment comment)
        {
            lock (_commentsLock)
            {
                _comments?.Remove(comment);
                _commentsById?.Remove(comment.Id);
            }
        }

        #endregion

        #region private methods

        private List<IComment> GetOrLoadComments()
        {
            lock (_commentsLock)
            {
                if (_comments == null)
                {
                    _commentsById = new Dictionary<string, IComment>();
                    _comments = LoadComments();
                }

                return _comments;
            }
        }

        private List<IComment> LoadComments()
        {
            var dataById = _repository.LoadComments(_page.Documentation.Id, _page.Id).ToDictionary(_ => _.Id);

            var comments = new Dictionary<string, Comment>();
            var rootLevelComments = new List<IComment>();
            var items = new List<ICommentData>();

            // Materialize root-level comments
            items.AddRange(dataById.Values.Where(_ => _.ReplyToId == null));
            foreach (var data in items.OrderBy(_ => _.CreatedTime))
            {
                var comment = new Comment(this, data);
                comments.Add(data.Id, comment);
                dataById.Remove(data.Id);
                rootLevelComments.Add(comment);
            }

            // Materialize reply comments
            while (dataById.Count > 0)
            {
                items.Clear();
                items.AddRange(dataById.Values);

                var hasAnyProcessedComments = false;
                foreach (var data in items.OrderBy(_ => _.CreatedTime))
                {
                    Comment parent;
                    if (comments.TryGetValue(data.ReplyToId, out parent))
                    {
                        var reply = parent.PreloadReply(data);
                        comments.Add(data.Id, reply);

                        dataById.Remove(data.Id);

                        hasAnyProcessedComments = true;
                    }
                }

                if (!hasAnyProcessedComments)
                {
                    _log.LogWarning("Page '{0}' has some unrecognized comments: {1}",
                        _page.Id,
                        string.Join(", ", from _ in dataById.Values select _.Id)
                    );
                    break;
                }
            }
            
            return rootLevelComments;
        }

        #endregion
    }
}