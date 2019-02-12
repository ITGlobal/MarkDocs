using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    /// <summary>
    ///     Compilation report model
    /// </summary>
    [PublicAPI]
    public sealed class CompilationReportModel
    {
        /// <summary>
        ///     Messages
        /// </summary>
        [JsonProperty("messages")]
        [NotNull]
        public CompilationReportMessageModel[] Messages { get; set; }
    }
}