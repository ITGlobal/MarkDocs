using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Comments.Data;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     A comment
    /// </summary>
    internal class Comment : IComment, ICommentParent
    {
        #region fields

        private readonly ICommentParent _parent;

        private readonly object _dataLock = new object();
        private ICommentData _data;

        private readonly object _repliesLock = new object();
        private readonly List<IReplyComment> _replies = new List<IReplyComment>();

        #endregion

        #region .ctor
        
        public Comment(ICommentParent parent, ICommentData data)
        {
            _parent = parent;
            _data = data;

            _parent.OnAdded(this);
        }

        #endregion

        #region IComment

        /// <summary>
        ///     Comment ID
        /// </summary>
        public string Id => GetProperty(_ => _.Id);

        /// <summary>
        ///     Comment author ID
        /// </summary>
        public string UserId => GetProperty(_ => _.UserId);

        /// <summary>
        ///     Parent page
        /// </summary>
        public IPage Page => _parent.Page;

        /// <summary>
        ///     Date and time when comment has been created
        /// </summary>
        public DateTime CreatedTime => GetProperty(_ => _.CreatedTime);

        /// <summary>
        ///     Date and time when comment has been created or edited
        /// </summary>
        public DateTime LastChangeTime => GetProperty(_ => _.LastChangeTime);

        /// <summary>
        ///     True if a comment has been edited
        /// </summary>
        public bool IsEdited => GetProperty(_ => _.IsEdited);

        /// <summary>
        ///     Comment markup
        /// </summary>
        public string Markup => GetProperty(_ => _.Markup);

        /// <summary>
        ///     Comment rendered markup
        /// </summary>
        public string Html => RenderMarkup(GetProperty(_ => _.Markup));

        /// <summary>
        ///     List of replies
        /// </summary>
        public IReadOnlyList<IReplyComment> Replies
        {
            get
            {
                lock (_repliesLock)
                {
                    return _replies.ToList();
                }
            }
        }

        /// <summary>
        ///     Adds a reply
        /// </summary>
        /// <param name="user">
        ///     User ID
        /// </param>
        /// <param name="markup">
        ///     A reply markup
        /// </param>
        /// <returns>
        ///     A reply
        /// </returns>
        public IReplyComment Reply(string user, string markup)
        {
            lock (_repliesLock)
            {
                var data = _parent.Repository.CreateComment(_parent.Page.Documentation.Id, _parent.Page.Id, user, markup);
                var reply = new ReplyComment(this, data);
                _replies.Add(reply);
                return reply;
            }
        }

        /// <summary>
        ///     Edits a comment
        /// </summary>
        /// <param name="markup">
        ///     Comment edited markup
        /// </param>
        public void Edit(string markup)
        {
            lock (_dataLock)
            {
                _data = _parent.Repository.EditComment(_data.Id, markup);
            }
        }

        /// <summary>
        ///     Deletes a comment
        /// </summary>
        public void Delete()
        {
            _parent.Repository.DeleteComment(_data.Id);
            _parent.OnDeleted(this);
        }

        #endregion

        #region ICommentParent

        IPage ICommentParent.Page => Page;
        ICommentDataRepository ICommentParent.Repository => _parent.Repository;
        IFormat ICommentParent.Format => _parent.Format;

        void ICommentParent.OnAdded(IComment comment) => _parent.OnAdded(comment);

        void ICommentParent.OnDeleted(IComment comment)
        {
            lock (_repliesLock)
            {
                _replies.Remove((IReplyComment)comment);
            }
        }

        #endregion

        #region internal methods
        
        internal ReplyComment PreloadReply(ICommentData data)
        {
            lock (_repliesLock)
            {
                var reply = new ReplyComment(this, data);
                _replies.Add(reply);
                return reply;
            }
        }

        #endregion

        #region private methods

        private T GetProperty<T>(Func<ICommentData, T> func)
        {
            lock (_dataLock)
            {
                return func(_data);
            }
        }

        private string RenderMarkup(string markup)
        {
            var html = _parent.Format.Render(_parent.Page, markup);
            return html;
        }

        #endregion
    }
}