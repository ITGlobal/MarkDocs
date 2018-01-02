using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog attachment
    /// </summary>
    public interface IBlogAttachment : IBlogResource
    {
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