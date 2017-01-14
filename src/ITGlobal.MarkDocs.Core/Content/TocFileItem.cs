using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     "toc.json" file item model
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    internal sealed class TocFileItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        /// <summary>
        ///     Converts current object into <see cref="Metadata"/>
        /// </summary>
        public Metadata ToPageProperties()
        {
            var properties = new Metadata
            {
                Title = Title,
                Order = Order,
                Tags = Tags
            };
            
            return properties;
        }
    }
}