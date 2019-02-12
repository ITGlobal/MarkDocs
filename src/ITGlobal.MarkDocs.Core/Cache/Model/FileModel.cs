using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    /// <summary>
    ///     A file resource model
    /// </summary>
    [PublicAPI]
    public sealed class FileModel : ResourceModel
    {
        /// <summary>
        ///     MIME type
        /// </summary>
        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        ///     File type
        /// </summary>
        [JsonProperty("type")]
        public FileType Type { get; set; }
    }
}