using ITGlobal.MarkDocs.Comments.Data;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     A reply to a comment
    /// </summary>
    internal sealed class ReplyComment : Comment, IReplyComment
    {
        public ReplyComment(Comment parent, ICommentData data)
            : base(parent, data)
        {
            ReplyTo = parent;
        }
        
        /// <summary>
        ///     A parent comment
        /// </summary>
        public IComment ReplyTo { get; }
    }
}