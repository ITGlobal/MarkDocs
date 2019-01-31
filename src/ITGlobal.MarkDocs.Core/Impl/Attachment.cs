using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Cache.Model;
using System.IO;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class Attachment : IAttachment
    {
        #region fields

        private readonly ICacheReader _cache;
        private readonly AttachmentModel _model;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public Attachment(IDocumentation documentation, ICacheReader cache, AttachmentModel model)
        {
            Documentation = documentation;
            _cache = cache;
            _model = model;

            switch (model.Type)
            {
                case AttachmentType.Generated:
                    Type = ResourceType.GeneratedAttachment;
                    break;
                default:
                    Type = ResourceType.Attachment;
                    break;
            }
        }

        #endregion

        #region IAttachment

        /// <summary>
        ///     Attachment path
        /// </summary>
        public string Id => _model.Id;

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        public IDocumentation Documentation { get; }

        /// <summary>
        ///     Attachment file name
        /// </summary>
        public string FileName => _model.FileName;

        /// <summary>
        ///     Attachment MIME type
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
