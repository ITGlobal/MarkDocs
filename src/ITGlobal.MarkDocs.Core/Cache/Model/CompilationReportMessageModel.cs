using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    /// <summary>
    ///     Compilation report message model
    /// </summary>
    [PublicAPI]
    public sealed class CompilationReportMessageModel
    {
        /// <summary>
        ///     Path to erroneous page file (relative to source root)
        /// </summary>
        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public string Filename { get; set; }

        /// <summary>
        ///     Erroneous page line number
        /// </summary>
        [JsonProperty("line", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public int? LineNumber { get; set; }

        /// <summary>
        ///     Compilation message severity
        /// </summary>
        [JsonProperty("type")]
        public CompilationReportMessageType Type { get; set; }

        /// <summary>
        ///     Message text
        /// </summary>
        [JsonProperty("msg")]
        [NotNull]
        public string Message { get; set; }
    }
}