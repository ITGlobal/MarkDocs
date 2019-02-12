using ITGlobal.MarkDocs.Cache.Model;
using JetBrains.Annotations;
using System;
using System.Linq;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     A page anchor
    /// </summary>
    [PublicAPI]
    public sealed class PageAnchor
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public PageAnchor(string id, string title, PageAnchor[] nested)
        {
            Id = id;
            Title = title;
            Nested = nested;
        }

        /// <summary>
        ///     Anchor ID (href)
        /// </summary>
        [NotNull]
        public string Id { get; }

        /// <summary>
        ///     Anchor title
        /// </summary>
        [NotNull]
        public string Title { get; }

        /// <summary>
        ///     Nested anchors
        /// </summary>
        [NotNull]
        public PageAnchor[] Nested { get; }

        /// <inheritdoc />
        public override string ToString() => $"{Title} {{#{Id}}}";

        [NotNull]
        internal PageAnchorModel ToModel()
        {
            return new PageAnchorModel
            {
                Id = Id,
                Title = Title,
                Nested = Nested != null && Nested.Length > 0
                    ? Nested.Select(_ => _.ToModel()).ToArray()
                    : null
            };
        }

        [NotNull]
        internal static PageAnchor FromModel([NotNull] PageAnchorModel model)
        {
            return new PageAnchor(
                id: model.Id,
                title: model.Title,
                nested: model.Nested != null
                    ? model.Nested.Select(FromModel).ToArray()
                    : Array.Empty<PageAnchor>()
            );
        }
    }
}