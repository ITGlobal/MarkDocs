using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A attachment
    /// </summary>
    [PublicAPI]
    public interface IFileResource : IResource
    {
        /// <summary>
        ///     FileResource MIME type
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