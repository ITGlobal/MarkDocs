using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    public sealed class CompilationReportMessageModel
    {
        [JsonProperty("page", NullValueHandling = NullValueHandling.Ignore)]
        public string Page { get; set; }

        [JsonProperty("type")]
        public CompilationReportMessageType Type { get; set; }

        [JsonProperty("line")]
        public int? LineNumber { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}