using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A attachment
    /// </summary>
    [PublicAPI]
    public interface IAttachment : IResource
    {
        /// <summary>
        ///     Attachment file name
        /// </summary>
        [PublicAPI, NotNull]
        string FileName { get; }

        /// <summary>
        ///     Attachment MIME type
        /// </summary>
        [PublicAPI, NotNull]
        string ContentType { get; }

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        [PublicAPI, NotNull]
        Stream Read();
    }
}