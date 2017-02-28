using System.Collections.Generic;
using System.Linq;
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

        [JsonProperty("meta")]
        public Dictionary<string, string> MetaTags { get; set; }

        /// <summary>
        ///     Converts current object into <see cref="Metadata"/>
        /// </summary>
        public Metadata ToPageProperties()
        {
            var properties = new Metadata
            {
                Title = Title,
                Order = Order
            };

            if (Tags != null)
            {
                properties.Tags = Tags;
            }
            if (MetaTags != null)
            {
                properties.MetaTags = MetaTags.Select(_ => new MetaTag {Name = _.Key, Content = _.Value}).ToArray();
            }
            
            return properties;
        }
    }
}