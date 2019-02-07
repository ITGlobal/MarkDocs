using JetBrains.Annotations;
using System.IO;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog attachment
    /// </summary>
    [PublicAPI]
    public interface IBlogAttachment : IBlogResource
    {
        /// <summary>
        ///     Attachment MIME type
        /// </summary>
        [NotNull]
        string ContentType { get; }

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        [NotNull]
        Stream OpenRead();
    }
}