using System.IO;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     Blog attachment
    /// </summary>
    internal sealed class BlogAttachment : IBlogAttachment
    {
        private readonly IFileResource _resource;

        public BlogAttachment(IBlogEngine engine, IFileResource resource)
        {
            Engine = engine;
            _resource = resource;

            switch (resource.Type)
            {
                case ResourceType.GeneratedFile:
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
        public string RelativePath => _resource.RelativePath;

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
        public Stream OpenRead() => _resource.OpenRead();
    }
}