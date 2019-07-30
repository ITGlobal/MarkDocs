using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Source.Impl
{
    /// <summary>
    ///     "toc.json" file item model
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    internal sealed class TocFileItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("short_title")]
        public string ShortTitle { get; set; }

        [JsonProperty("order")]
        public int? Order { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("meta")]
        public Dictionary<string, string> MetaTags { get; set; }

        /// <summary>
        ///     Converts current object into <see cref="PageMetadata"/>
        /// </summary>
        public PageMetadata GetMetadata()
        {
            return new PageMetadata(
                title: Title,
                shortTitle: ShortTitle,
                tags: Tags,
                order: Order,
                metaTags: MetaTags
                    ?.Select(p => $"<meta name=\"{p.Key}\" content=\"{HtmlEncoder.Default.Encode(p.Value)}\" />")
                    .ToArray()
            );
        }
    }
}