using System;
using System.Collections.Immutable;
using System.Linq;

namespace ITGlobal.MarkDocs.Tags.Impl
{
    internal sealed class TagsExtensionStateBranch
    {

        public TagsExtensionStateBranch(IDocumentation documentation)
        {
            var pagesBuilder = ImmutableDictionary.CreateBuilder<string, PageTagNode>(StringComparer.OrdinalIgnoreCase);

            Add(pagesBuilder, documentation.RootPage);

            Pages = pagesBuilder.ToImmutable();
            Tags = Pages.SelectMany(_ => _.Value.Tags).Distinct().ToImmutableArray();
        }

        public ImmutableArray<string> Tags { get; }
        public ImmutableDictionary<string, PageTagNode> Pages { get; }

        private static void Add(ImmutableDictionary<string, PageTagNode>.Builder pagesBuilder, IPage page)
        {
            pagesBuilder.Add(page.Id, new PageTagNode(page, page.Metadata.Tags));

            foreach (var nestedPage in page.NestedPages)
            {
                Add(pagesBuilder, nestedPage);
            }
        }

    }
}