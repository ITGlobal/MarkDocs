using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Cache.Model;
using System.IO;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class FileResource : IFileResource
    {
        #region fields

        private readonly ICacheReader _cache;
        private readonly FileModel _model;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public FileResource(IDocumentation documentation, ICacheReader cache, FileModel model)
        {
            Documentation = documentation;
            _cache = cache;
            _model = model;

            switch (model.Type)
            {
                case AttachmentType.Generated:
                    Type = ResourceType.GeneratedFile;
                    break;
                default:
                    Type = ResourceType.File;
                    break;
            }
        }

        #endregion

        #region IFileResource

        /// <summary>
        ///     FileResource path
        /// </summary>
        public string Id => _model.Id;

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        public IDocumentation Documentation { get; }

        /// <summary>
        ///     FileResource file name
        /// </summary>
        public string RelativePath => _model.RelativePath;

        /// <summary>
        ///     FileResource MIME type
        /// </summary>
        public string ContentType => _model.ContentType;

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        public Stream OpenRead() => _cache.Read(this);

        /// <summary>
        ///     Resource type
        /// </summary>
        public ResourceType Type { get; }

        #endregion
    }
}
