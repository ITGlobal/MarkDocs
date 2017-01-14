using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     A reply to a comment
    /// </summary>
    [PublicAPI]
    public interface IReplyComment : IComment
    {
        /// <summary>
        ///     A parent comment
        /// </summary>
        [PublicAPI, NotNull]
        IComment ReplyTo { get; }
    }
}