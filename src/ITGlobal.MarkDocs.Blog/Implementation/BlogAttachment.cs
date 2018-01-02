using System.IO;

namespace ITGlobal.MarkDocs.Blog.Implementation
{
    /// <summary>
    ///     Blog attachment
    /// </summary>
    internal sealed class BlogAttachment : IBlogAttachment
    {
        private readonly IAttachment _resource;

        public BlogAttachment(IBlogEngine engine, IAttachment resource)
        {
            Engine = engine;
            _resource = resource;

            switch (resource.Type)
            {
                case ResourceType.Illustration:
                    Type = BlogResourceType.Illustration;
                    break;
                default:
                    Type = BlogResourceType.Attachment;
                    break;
            }
        }

        /// <summary>
        ///     Resource ID
        /// </summary>
        public string Id => _resource.Id;

        /// <summary>
        ///     A reference to a blog engine
        /// </summary>
        public IBlogEngine Engine { get; }

        /// <summary>
        ///     Name of resource file with extension (only name, not a full path)
        /// </summary>
        public string FileName => _resource.FileName;

        /// <summary>
        ///     Attachment MIME type
        /// </summary>
        public string ContentType => _resource.ContentType;

        /// <summary>
        ///     Resource type
        /// </summary>
        public BlogResourceType Type { get; }

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        public Stream Read() => _resource.Read();
    }
}