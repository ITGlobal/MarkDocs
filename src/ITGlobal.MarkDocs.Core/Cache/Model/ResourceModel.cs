using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    /// <summary>
    ///     A resource model
    /// </summary>
    [PublicAPI]
    public abstract class ResourceModel
    {
        /// <summary>
        ///     Resource ID
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Path to resource file (relative to source root)
        /// </summary>
        [JsonProperty("filename")]
        public string RelativePath { get; set; }
    }
}