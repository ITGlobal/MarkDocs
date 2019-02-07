using System;
using System.Linq;
using ITGlobal.MarkDocs.Cache.Model;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    public sealed class PageAnchor
    {
        public PageAnchor(string id, string title, PageAnchor[] nested)
        {
            Id = id;
            Title = title;
            Nested = nested;
        }
        
        [NotNull]
        public string Id { get; }

        [NotNull]
        public string Title { get; }

        [NotNull]
        public PageAnchor[] Nested { get; }

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
                model.Nested != null
                    ? model.Nested.Select(FromModel).ToArray()
                    : Array.Empty<PageAnchor>()
            );
        }
    }
}