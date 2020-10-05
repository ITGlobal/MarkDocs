using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ITGlobal.MarkDocs.Tags.Impl
{
    internal sealed class PageTagNode
    {

        private readonly HashSet<string> _tags;

        public PageTagNode(IPage page, string[] tags)
        {
            Page = page;
            _tags = new HashSet<string>(
                (tags ?? Array.Empty<string>()).Select(TagsExtension.NormalizeTag),
                StringComparer.OrdinalIgnoreCase
            );
            Tags = _tags.ToImmutableArray();
        }

        public IPage Page { get; }
        public ImmutableArray<string> Tags { get; }

        public bool HasTag(string tag) => _tags.Contains(tag);

    }
}