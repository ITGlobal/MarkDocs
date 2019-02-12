using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ITGlobal.MarkDocs.Cache.Model
{
    /// <summary>
    ///     File type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FileType
    {
        /// <summary>
        ///     A physical file
        /// </summary>
        [EnumMember(Value = "file")]
        File,

        /// <summary>
        ///     A generated file
        /// </summary>
        [EnumMember(Value = "generated")]
        Generated
    }
}