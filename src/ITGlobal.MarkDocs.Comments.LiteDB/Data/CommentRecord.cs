using System;
using LiteDB;

namespace ITGlobal.MarkDocs.Comments.Data
{
    /// <summary>
    ///     A comment data
    /// </summary>
    internal sealed class CommentRecord : ICommentData
    {
        public const string Collection = "comments";

        /// <summary>
        ///     ID
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        ///     Comment ID
        /// </summary>
        [BsonIgnore]
        string ICommentData.Id => Id.ToString();

        /// <summary>
        ///     Parent comment ID. null for root-level comments.
        /// </summary>
        [BsonField("reply_to")]
        public ObjectId ReplyToId { get; set; } = ObjectId.Empty;

        /// <summary>
        ///     Parent comment ID. null for root-level comments.
        /// </summary>
        [BsonIgnore]
        string ICommentData.ReplyToId => ReplyToId != ObjectId.Empty ? ReplyToId?.ToString() : null;

        /// <summary>
        ///     Page ID
        /// </summary>
        [BsonField("page"), BsonRef(PageRecord.Collection)]
        public PageRecord Page { get; set; }

        /// <summary>
        ///     User ID
        /// </summary>
        [BsonField("user_id")]
        public string UserId { get; set; }

        /// <summary>
        ///     Creation date and time
        /// </summary>
        [BsonField("created")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        ///     Last change's date and time
        /// </summary>
        [BsonField("changed")]
        public DateTime LastChangeTime { get; set; }

        /// <summary>
        ///     true if comment has been edited
        /// </summary>
        [BsonField("is_edited")]
        public bool IsEdited { get; set; }

        /// <summary>
        ///     Comment markup
        /// </summary>
        [BsonField("text")]
        public string Markup { get; set; }

        public static LiteCollection<CommentRecord> GetCollection(LiteDatabase db)
        {
            var collection = db.GetCollection<CommentRecord>(Collection);
            collection.EnsureIndex(_ => _.UserId);
            collection.EnsureIndex(_ => _.CreatedTime);
            return collection;
        }
    }
}