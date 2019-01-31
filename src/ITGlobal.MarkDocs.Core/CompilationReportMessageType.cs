using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Compilation message severity
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CompilationReportMessageType
    {
        /// <summary>
        ///     Warning
        /// </summary>
        [EnumMember(Value = "warning")]
        Warning,

        /// <summary>
        ///     Error
        /// </summary>
        [EnumMember(Value = "error")]
        Error
    }
}