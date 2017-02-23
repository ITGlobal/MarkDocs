using System;
using System.IO;

namespace ITGlobal.MarkDocs.Comments
{
    internal sealed class CommentAttachment : IAttachment
    {
        private readonly byte[] _content;

        public CommentAttachment(IDocumentation documentation, string fileName, byte[] content)
        {
            Id = Guid.NewGuid().ToString("N");
            Documentation = documentation;
            FileName = fileName;
            _content = content;
        }

        /// <summary>
        ///     Attachment ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        public IDocumentation Documentation { get; }

        /// <summary>
        ///     Attachment file name
        /// </summary>
        public string FileName { get; }

        /// <summary>
        ///     Attachment MIME type
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        public Stream Read() => new MemoryStream(_content);
    }
}