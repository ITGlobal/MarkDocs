using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    public sealed class CompilationReportMessageModel
    {
        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        public string Filename { get; set; }

        [JsonProperty("line", NullValueHandling = NullValueHandling.Ignore)]
        public int? LineNumber { get; set; }

        [JsonProperty("type")]
        public CompilationReportMessageType Type { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}