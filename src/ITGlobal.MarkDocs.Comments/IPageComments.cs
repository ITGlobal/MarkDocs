using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     Comments for a page
    /// </summary>
    [PublicAPI]
    public interface IPageComments
    {
        /// <summary>
        ///     List of comments
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IComment> Comments { get; }

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
        [PublicAPI, NotNull]
        IComment AddComment([NotNull] string user, [NotNull] string markup);

        /// <summary>
        ///     Find a comment or reply by its ID
        /// </summary>
        /// <param name="id">
        ///     Comment ID
        /// </param>
        /// <returns>
        ///     A comment or null
        /// </returns>
        [PublicAPI, CanBeNull]
        IComment FindComment([NotNull] string id);
    }
}