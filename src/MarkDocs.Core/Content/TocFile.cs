using System.Collections.Generic;
using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     "toc.json" file model
    /// </summary>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    internal sealed class TocFile
    {
        /// <summary>
        ///     "toc.json" file items
        /// </summary>
        [JsonProperty("pages")]
        public Dictionary<string, TocFileItem> Items { get; set; } = new Dictionary<string, TocFileItem>();

        /// <summary>
        ///     Gets page metadata if available
        /// </summary>
        public Metadata TryGetMetadata(string filename)
        {
            TocFileItem item;
            if (!Items.TryGetValue(filename, out item))
            {
                return null;
            }

            return item.ToPageProperties();
        }
    }
}