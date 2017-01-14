using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Comments.Data
{
    /// <summary>
    ///     A comment data storage
    /// </summary>
    [PublicAPI]
    public interface ICommentDataRepository
    {
        /// <summary>
        ///     Loads list of all page IDs that have any comments
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<string> LoadPageIds([NotNull] string documentationId);

        /// <summary>
        ///     Loads a flat list of page comments and replies
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<ICommentData> LoadComments([NotNull] string documentationId, [NotNull] string pageId);

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
        [PublicAPI, NotNull]
        ICommentData CreateComment([NotNull] string documentationId, [NotNull] string pageId, [NotNull] string user, [NotNull] string markup);

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
        [PublicAPI, NotNull]
        ICommentData CreateReply([NotNull] string documentationId, [NotNull] string pageId, [NotNull] string replyToId, [NotNull] string user, [NotNull] string markup);

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
        [PublicAPI, NotNull]
        ICommentData EditComment([NotNull] string id, [NotNull] string markup);

        /// <summary>
        ///     Deletes a comment
        /// </summary>
        /// <param name="id">
        ///     Comment ID
        /// </param>
        [PublicAPI]
        void DeleteComment([NotNull] string id);

        /// <summary>
        ///     Deletes all page comments
        /// </summary>
        /// <param name="documentationId">
        ///     Documentation Id
        /// </param>
        /// <param name="pageId">
        ///     Page ID
        /// </param>
        [PublicAPI]
        bool DeletePageComments([NotNull] string documentationId, [NotNull] string pageId);
    }
}