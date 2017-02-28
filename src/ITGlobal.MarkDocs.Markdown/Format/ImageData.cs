using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Rendered image
    /// </summary>
    [PublicAPI]
    public sealed class ImageData
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public ImageData(byte[] content, string filetype = ".png")
        {
            Content = content;
            FileType = filetype;
        }

        /// <summary>
        ///     File type (e.g. ".png")
        /// </summary>
        [PublicAPI, NotNull]
        public string FileType { get; }

        /// <summary>
        ///     File content
        /// </summary>
        [PublicAPI, NotNull]
        public byte[] Content { get; }
    }
}