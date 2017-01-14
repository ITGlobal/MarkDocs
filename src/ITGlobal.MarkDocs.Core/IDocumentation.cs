using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A documentation
    /// </summary>
    [PublicAPI]
    public interface IDocumentation
    {
        /// <summary>
        ///     Documentation service
        /// </summary>
        [PublicAPI, NotNull]
        IMarkDocService Service { get; }

        /// <summary>
        ///     Documentation ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }

        /// <summary>
        ///     Documentation title
        /// </summary>
        [PublicAPI, NotNull]
        string Title { get; }

        /// <summary>
        ///     Documentation version
        /// </summary>
        [PublicAPI, NotNull]
        IContentVersion ContentVersion { get; }

        /// <summary>
        ///     Page tree
        /// </summary>
        [PublicAPI, NotNull]
        IPageTree PageTree { get; }

        /// <summary>
        ///     Documentation attached files
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IAttachment> Attachments { get; }

        /// <summary>
        ///     Gets a documentation page by its ID
        /// </summary>
        /// <param name="id">
        ///     Page ID
        /// </param>
        /// <returns>
        ///     A documentation page or null if page doesn't exist
        /// </returns>
        [PublicAPI, CanBeNull]
        IPage GetPage([NotNull] string id);

        /// <summary>
        ///     Gets an attachment by its ID
        /// </summary>
        /// <param name="id">
        ///     Attachment path
        /// </param>
        /// <returns>
        ///     An attachment or null if it doesn't exist
        /// </returns>
        [PublicAPI, CanBeNull]
        IAttachment GetAttachment([NotNull] string id);
    }
}