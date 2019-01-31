using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal sealed class DiskCacheFileDescriptor
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }
    }
}