using System;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    public sealed class SourceInfoModel
    {
        [JsonProperty("source_url", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceUrl { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonProperty("last_change_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? LastChangeTime { get; set; }

        [JsonProperty("last_change_id", NullValueHandling = NullValueHandling.Ignore)]
        public string LastChangeId { get; set; }

        [JsonProperty("last_change_desc", NullValueHandling = NullValueHandling.Ignore)]
        public string LastChangeDescription { get; set; }

        [JsonProperty("last_change_author", NullValueHandling = NullValueHandling.Ignore)]
        public string LastChangeAuthor { get; set; }
    }
}