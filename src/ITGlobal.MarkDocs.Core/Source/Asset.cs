using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Asset
    /// </summary>
    [PublicAPI]
    public abstract class Asset : IResourceId
    {
        /// <summary>
        ///     Default MIME type
        /// </summary>
        public const string DEFAULT_MIME_TYPE = "application/octet-stream";

        /// <summary>
        ///     .ctor
        /// </summary>
        internal Asset(string id)
        {
            Id = id;
        }

        /// <summary>
        ///     Resource ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Resource type
        /// </summary>
        public abstract ResourceType Type { get; }
    }
}
