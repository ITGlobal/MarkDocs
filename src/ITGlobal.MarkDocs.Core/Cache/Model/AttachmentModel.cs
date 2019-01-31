using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    public sealed class AttachmentModel : ResourceModel
    {
        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("type")]
        public AttachmentType Type { get; set; }
    }
}