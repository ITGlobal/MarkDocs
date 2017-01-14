using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace ITGlobal.MarkDocs.Comments.Data
{
    /// <summary>
    ///     LiteDB-based comment data storage
    /// </summary>
    internal sealed class LiteDbCommentDataRepository : ICommentDataRepository
    {
        #region fields

        private readonly string _dbConnectionString;
        private readonly BsonMapper _bsonMapper;
        private readonly IReadOnlyList<ICommentData> _emptyList = new ICommentData[0];

        #endregion

        #region .ctor

        public LiteDbCommentDataRepository(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
            _bsonMapper = new BsonMapper
            {
                SerializeNullValues = true,
                TrimWhitespace = true,
                EmptyStringToNull = false
            };
        }

        #endregion

        #region ICommentDataRepository

        /// <summary>
        ///     Loads list of all page IDs that have any comments
        /// </summary>
        public IReadOnlyList<string> LoadPageIds(string documentationId)
        {
            return Exec(false, db =>
            {
                var collection = PageRecord.GetCollection(db);
                var pageIds = collection.Find(_ => _.DocumentationId == documentationId).Select(_ => _.PageId).ToArray();
                return pageIds;
            });
        }

        /// <summary>
        ///     Loads a flat list of page comments and replies
        /// </summary>
        public IReadOnlyList<ICommentData> LoadComments(string documentationId, string pageId)
        {
            return Exec(false, db =>
            {
                var page = PageRecord.Get(db, documentationId, pageId);
                if (page == null)
                {
                    return _emptyList;
                }

                var comments = CommentRecord.GetCollection(db);
                var items = comments.Find(_ => _.Page.Id == page.Id).Cast<ICommentData>().ToArray();
                return items;
            });
        }

        /// <summary>
        ///     Creates new root-level comment
        /// </summary>
        /// <param name="documentationId">
        ///     Documentation Id
        /// </param>
        /// <param name="pageId">
        ///     Page ID
        /// </param>
        /// <param name="user">
        ///     User ID
        /// </param>
        /// <param name="markup">
        ///     Comment markup
        /// </param>
        /// <returns>
        ///     Comment data
        /// </returns>
        public ICommentData CreateComment(string documentationId, string pageId, string user, string markup)
        {
            return Exec(true, db =>
            {
                var page = PageRecord.GetOrCreate(db, documentationId, pageId);

                var comments = CommentRecord.GetCollection(db);
                var comment = new CommentRecord
                {
                    CreatedTime = DateTime.Now,
                    LastChangeTime = DateTime.Now,
                    Page = page,
                    UserId = user,
                    Markup = markup
                };
                comments.Insert(comment);
                return (ICommentData)comment;
            });
        }

        /// <summary>
        ///     Creates new reply comment
        /// </summary>
        /// <param name="documentationId">
        ///     Documentation Id
        /// </param>
        /// <param name="pageId">
        ///     Page ID
        /// </param>
        /// <param name="replyToId">
        ///     Parent comment ID
        /// </param>
        /// <param name="user">
        ///     User ID
        /// </param>
        /// <param name="markup">
        ///     Comment markup
        /// </param>
        /// <returns>
        ///     Comment data
        /// </returns>
        public ICommentData CreateReply(string documentationId, string pageId, string replyToId, string user,
            string markup)
        {
            return Exec(true, db =>
            {
                var page = PageRecord.GetOrCreate(db, documentationId, pageId);

                var comments = CommentRecord.GetCollection(db);
                var replyTo = comments.FindById(new ObjectId(replyToId));
                if (replyTo == null)
                {
                    throw new Exception($"Unable to find comment '{replyToId}'");
                }

                var comment = new CommentRecord
                {
                    CreatedTime = DateTime.Now,
                    LastChangeTime = DateTime.Now,
                    ReplyToId = replyTo.Id,
                    Page = page,
                    UserId = user,
                    Markup = markup
                };
                comments.Insert(comment);
                return (ICommentData)comment;
            });
        }

        /// <summary>
        ///     Edits a comment
        /// </summary>
        /// <param name="id">
        ///     Comment ID
        /// </param>
        /// <param name="markup">
        ///     Comment markup
        /// </param>
        /// <returns>
        ///     Comment data
        /// </returns>
        public ICommentData EditComment(string id, string markup)
        {
            return Exec(true, db =>
            {
                var comments = CommentRecord.GetCollection(db);
                var commentId = new ObjectId(id);
                var comment = comments.FindOne(_ => _.Id == commentId);
                if (comment == null)
                {
                    throw new Exception($"Unable to find comment '{id}'");
                }

                comment.IsEdited = true;
                comment.LastChangeTime = DateTime.Now;
                comment.Markup = markup;
                comments.Update(comment);

                return (ICommentData)comment;
            });
        }

        /// <summary>
        ///     Deletes a comment
        /// </summary>
        /// <param name="id">
        ///     Comment ID
        /// </param>
        public void DeleteComment(string id)
        {
            Exec(true, db =>
            {
                var collection = CommentRecord.GetCollection(db);
                var commentId = new ObjectId(id);

                var count = collection.Delete(_ => _.Id == commentId);
                if (count == 0)
                {
                    throw new Exception($"Unable to find comment '{id}'");
                }

                DeleteReplies(collection, commentId);
            });
        }

        private void DeleteReplies(LiteCollection<CommentRecord> collection, ObjectId id)
        {
            var comments = collection.Find(_ => _.ReplyToId == id).ToArray();
            foreach (var comment in comments)
            {
                DeleteReplies(collection, comment.Id);
            }

            collection.Delete(_ => _.ReplyToId == id);
        }

        /// <summary>
        ///     Deletes all page comments
        /// </summary>
        /// <param name="documentationId">
        ///     Documentation Id
        /// </param>
        /// <param name="pageId">
        ///     Page ID
        /// </param>
        public bool DeletePageComments(string documentationId, string pageId)
        {
            return Exec(true, db =>
            {
                var page = PageRecord.Get(db, documentationId, pageId);
                if (page == null)
                {
                    return false;
                }

                var comments = CommentRecord.GetCollection(db);
                var count = comments.Delete(_ => _.Page.Id == page.Id);

                var pages = PageRecord.GetCollection(db);
                pages.Delete(_ => _.Id == page.Id);

                return count > 0;
            });
        }

        #endregion

        #region private methods

        private T Exec<T>(bool enableTransaction, Func<LiteDatabase, T> func)
        {
            using (var db = new LiteDatabase(_dbConnectionString, _bsonMapper))
            {
                using (var transaction = enableTransaction ? db.BeginTrans() : null)
                {
                    try
                    {
                        var result = func(db);
                        transaction?.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        transaction?.Rollback();
                        throw;
                    }
                }
            }
        }

        private void Exec(bool enableTransaction, Action<LiteDatabase> func)
        {
            Exec(enableTransaction, db =>
            {
                func(db);
                return (object)null;
            });
        }

        #endregion
    }
}