using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Disk cache descriptor item
    /// </summary>
    internal sealed class DiskCacheDocumentationDescriptor
    {
        /// <summary>
        ///     A relative path to a cache subdirectory
        /// </summary>
        [JsonProperty("directory")]
        public string Directory { get; set; }

        /// <summary>
        ///     Cache subdirectory source version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}