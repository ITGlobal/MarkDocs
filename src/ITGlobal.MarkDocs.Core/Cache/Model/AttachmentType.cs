using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ITGlobal.MarkDocs.Cache.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AttachmentType
    {
        [EnumMember(Value = "file")]
        File,

        [EnumMember(Value = "generated")]
        Generated
    }
}