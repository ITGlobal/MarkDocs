using JetBrains.Annotations;
using System;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Page metadata
    /// </summary>
    [PublicAPI]
    public sealed class PageMetadata
    {
        #region fields

        /// <summary>
        ///     Default value
        /// </summary>
        public static readonly PageMetadata Empty = new PageMetadata();

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public PageMetadata(
            string contentId = null,
            string lastChangedBy = null,
            string title = null,
            string description = null,
            int? order = null,
            string[] tags = null,
            string[] metaTags = null,
            ImmutableDictionary<string, string> properties=null)
        {
            ContentId = contentId ?? "";
            LastChangedBy = lastChangedBy ?? "";
            Title = title ?? "";
            Description = description ?? "";
            Order = order;
            Tags = tags ?? Array.Empty<string>();
            MetaTags = metaTags ?? Array.Empty<string>();
            Properties = properties ?? ImmutableDictionary<string, string>.Empty;
        }

        #endregion

        #region metadata

        /// <summary>
        ///     Page permanent identifier
        /// </summary>
        [JsonProperty("content_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [NotNull]
        public string ContentId { get; }

        /// <summary>
        ///     Page's last change author
        /// </summary>
        [JsonProperty("last_changed_by", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [NotNull]
        public string LastChangedBy { get; }

        /// <summary>
        ///     Page title
        /// </summary>
        [JsonProperty("title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [NotNull]
        public string Title { get; }

        /// <summary>
        ///     Page description
        /// </summary>
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [NotNull]
        public string Description { get; }

        /// <summary>
        ///     Page order
        /// </summary>
        [JsonProperty("order", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [CanBeNull]
        public int? Order { get; }

        /// <summary>
        ///     Page tags
        /// </summary>
        [JsonProperty("tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [NotNull]
        public string[] Tags { get; }

        /// <summary>
        ///     Page meta tags (HTML)
        /// </summary>
        [JsonProperty("meta_tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [NotNull]
        public string[] MetaTags { get; }

        /// <summary>
        ///     Custom properties
        /// </summary>
        [JsonProperty("props", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [NotNull]
        public ImmutableDictionary<string, string> Properties { get; }

        #endregion

        #region methods

        /// <summary>
        ///     Updates the <see cref="ContentId"/> property
        /// </summary>
        [NotNull]
        public PageMetadata WithContentId([NotNull] string value)
        {
            if (ContentId == value)
            {
                return this;
            }

            return new PageMetadata(
                contentId: value,
                lastChangedBy: LastChangedBy,
                title: Title,
                description: Description,
                order: Order,
                tags: Tags,
                metaTags: MetaTags,
                properties: Properties
            );
        }

        /// <summary>
        ///     Updates the <see cref="LastChangedBy"/> property
        /// </summary>
        [NotNull]
        public PageMetadata WithLastChangedBy(string value)
        {
            if (LastChangedBy == value)
            {
                return this;
            }

            return new PageMetadata(
                contentId: ContentId,
                lastChangedBy: value,
                title: Title,
                description: Description,
                order: Order,
                tags: Tags,
                metaTags: MetaTags,
                properties: Properties
            );
        }

        /// <summary>
        ///     Updates the <see cref="Order"/> property
        /// </summary>
        [NotNull]
        public PageMetadata WithOrder([CanBeNull] int? value)
        {
            if (Order == value)
            {
                return this;
            }

            return new PageMetadata(
                contentId: ContentId,
                lastChangedBy: LastChangedBy,
                title: Title,
                description: Description,
                order: value,
                tags: Tags,
                metaTags: MetaTags,
                properties: Properties
            );
        }

        /// <summary>
        ///     Updates the <see cref="Description"/> property
        /// </summary>
        [NotNull]
        public PageMetadata WithDescription([NotNull] string value)
        {
            if (Description == value)
            {
                return this;
            }

            return new PageMetadata(
                contentId: ContentId,
                lastChangedBy: LastChangedBy,
                title: Title,
                description: value,
                order: Order,
                tags: Tags,
                metaTags: MetaTags,
                properties: Properties
            );
        }

        /// <summary>
        ///     Updates the <see cref="Title"/> property
        /// </summary>
        [NotNull]
        public PageMetadata WithTitle([NotNull] string value)
        {
            if (Title == value)
            {
                return this;
            }

            return new PageMetadata(
                contentId: ContentId,
                lastChangedBy: LastChangedBy,
                title: value,
                description: Description,
                order: Order,
                tags: Tags,
                metaTags: MetaTags,
                properties: Properties
            );
        }

        /// <summary>
        ///     Updates the <see cref="Tags"/> property
        /// </summary>
        [NotNull]
        public PageMetadata WithTags([NotNull] string[] value)
        {
            return new PageMetadata(
                contentId: ContentId,
                lastChangedBy: LastChangedBy,
                title: Title,
                description: Description,
                order: Order,
                tags: value,
                metaTags: MetaTags,
                properties: Properties
            );
        }

        /// <summary>
        ///     Updates the <see cref="MetaTags"/> property
        /// </summary>
        [NotNull]
        public PageMetadata WithMetaTags([NotNull] string[] value)
        {
            return new PageMetadata(
                contentId: ContentId,
                lastChangedBy: LastChangedBy,
                title: Title,
                description: Description,
                order: Order,
                tags: Tags,
                metaTags: value,
                properties: Properties
            );
        }

        /// <summary>
        ///     Updates the <see cref="Properties"/> property
        /// </summary>
        [NotNull]
        public PageMetadata With([NotNull] string key, [NotNull] string value)
        {
            return new PageMetadata(
                contentId: ContentId,
                lastChangedBy: LastChangedBy,
                title: Title,
                description: Description,
                order: Order,
                tags: Tags,
                metaTags: MetaTags,
                properties: Properties.SetItem(key, value)
            );
        }

        /// <summary>
        ///     Merges two values of <see cref="PageMetadata"/> together
        /// </summary>
        [NotNull]
        public PageMetadata MergeWith([CanBeNull] PageMetadata other)
        {
            if (other == null || ReferenceEquals(other, Empty))
            {
                return this;
            }

            if (ReferenceEquals(this, Empty))
            {
                return other;
            }

            return new PageMetadata(
                contentId: MergeString(ContentId, other.ContentId),
                lastChangedBy: MergeString(LastChangedBy, other.LastChangedBy),
                title: MergeString(Title, other.Title),
                description: MergeString(Description, other.Description),
                order: Order ?? other.Order,
                tags: MergeArray(Tags, other.Tags),
                metaTags: MergeArray(MetaTags, other.MetaTags),
                properties: MergeDict(Properties, other.Properties)
            );

            string MergeString(string thisValue, string otherValue)
            {
                return !string.IsNullOrEmpty(thisValue) ? thisValue : otherValue;
            }

            string[] MergeArray(string[] thisValue, string[] otherValue)
            {
                if (thisValue == null || thisValue.Length == 0)
                {
                    return otherValue;
                }

                if (otherValue == null || otherValue.Length == 0)
                {
                    return thisValue;
                }

                return thisValue.Concat(otherValue).Distinct().ToArray();
            }

            ImmutableDictionary<string, string> MergeDict(
                ImmutableDictionary<string, string> thisValue,
                ImmutableDictionary<string, string> otherValue)
            {
                if (thisValue == null || thisValue.Count == 0)
                {
                    return otherValue;
                }

                if (otherValue == null || otherValue.Count == 0)
                {
                    return thisValue;
                }

                var builder = ImmutableDictionary.CreateBuilder<string, string>();
                foreach (var (k, v) in thisValue)
                {
                    builder[k] = v;
                }

                foreach (var (k, v) in otherValue)
                {
                    builder[k] = v;
                }

                return builder.ToImmutable();
            }
        }

        /// <summary>
        ///     Gets a custom string property by its key
        /// </summary>
        [CanBeNull]
        public string GetString([NotNull] string key)
        {
            if (!Properties.TryGetValue(key, out var value))
            {
                return null;
            }

            return value;
        }

        /// <inheritdoc />
        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);

        #endregion
    }
}