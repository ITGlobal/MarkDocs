using JetBrains.Annotations;
using Newtonsoft.Json;
using System;

namespace ITGlobal.MarkDocs.Cache.Model
{
    /// <summary>
    ///     Source info model
    /// </summary>
    [PublicAPI]
    public sealed class SourceInfoModel
    {
        /// <summary>
        ///     Source URL
        /// </summary>
        [JsonProperty("source_url", NullValueHandling = NullValueHandling.Ignore)]
        [NotNull]
        public string SourceUrl { get; set; }

        /// <summary>
        ///     Version
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public string Version { get; set; }

        /// <summary>
        ///     Last content change time
        /// </summary>
        [JsonProperty("last_change_time", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public DateTime? LastChangeTime { get; set; }

        /// <summary>
        ///     Last content change identifier
        /// </summary>
        [JsonProperty("last_change_id", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public string LastChangeId { get; set; }

        /// <summary>
        ///     Last content change description
        /// </summary>
        [JsonProperty("last_change_desc", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public string LastChangeDescription { get; set; }

        /// <summary>
        ///     Last content change author
        /// </summary>
        [JsonProperty("last_change_author", NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull]
        public string LastChangeAuthor { get; set; }
    }
}