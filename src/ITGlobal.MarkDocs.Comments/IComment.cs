using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     A comment
    /// </summary>
    [PublicAPI]
    public interface IComment
    {
        /// <summary>
        ///     Comment ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }

        /// <summary>
        ///     Comment author ID
        /// </summary>
        [PublicAPI, NotNull]
        string UserId { get; }

        /// <summary>
        ///     Parent page
        /// </summary>
        [PublicAPI, NotNull]
        IPage Page { get; }

        /// <summary>
        ///     Date and time when comment has been created
        /// </summary>
        [PublicAPI]
        DateTime CreatedTime { get; }

        /// <summary>
        ///     Date and time when comment has been created or edited
        /// </summary>
        [PublicAPI]
        DateTime LastChangeTime { get; }

        /// <summary>
        ///     True if a comment has been edited
        /// </summary>
        [PublicAPI]
        bool IsEdited { get; }

        /// <summary>
        ///     Comment markup
        /// </summary>
        [PublicAPI, NotNull]
        string Markup { get; }

        /// <summary>
        ///     Comment rendered markup
        /// </summary>
        [PublicAPI, NotNull]
        string Html { get; }

        /// <summary>
        ///     List of replies
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IReplyComment> Replies { get; }

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
        [PublicAPI, NotNull]
        IReplyComment Reply([NotNull] string user, [NotNull] string markup);

        /// <summary>
        ///     Edits a comment
        /// </summary>
        /// <param name="markup">
        ///     Comment edited markup
        /// </param>
        [PublicAPI]
        void Edit([NotNull] string markup);

        /// <summary>
        ///     Deletes a comment
        /// </summary>
        [PublicAPI]
        void Delete();
    }
}