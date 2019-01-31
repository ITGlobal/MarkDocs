using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    public sealed class CompilationReportModel
    {
        [JsonProperty("messages")]
        public CompilationReportMessageModel[] Messages { get; set; }
    }
}