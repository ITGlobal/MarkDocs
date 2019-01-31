using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Source.Impl
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
        public PageMetadata TryGetMetadata(string filename)
        {
            if (!Items.TryGetValue(filename, out var item))
            {
                return null;
            }

            return item.GetMetadata();
        }
    }
}