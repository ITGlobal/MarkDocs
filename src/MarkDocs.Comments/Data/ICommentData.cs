using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Comments.Data
{
    /// <summary>
    ///     A comment data
    /// </summary>
    [PublicAPI]
    public interface ICommentData
    {
        /// <summary>
        ///     Comment ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }

        /// <summary>
        ///     Parent comment ID. null for root-level comments.
        /// </summary>
        [PublicAPI, CanBeNull]
        string ReplyToId { get; }

        /// <summary>
        ///     User ID
        /// </summary>
        [PublicAPI, NotNull]
        string UserId { get; }

        /// <summary>
        ///     Creation date and time
        /// </summary>
        [PublicAPI]
        DateTime CreatedTime { get; }

        /// <summary>
        ///     Last change's date and time
        /// </summary>
        [PublicAPI]
        DateTime LastChangeTime { get; }

        /// <summary>
        ///     true if comment has been edited
        /// </summary>
        [PublicAPI]
        bool IsEdited { get; }

        /// <summary>
        ///     Comment markup
        /// </summary>
        [PublicAPI, NotNull]
        string Markup { get; }
    }
}