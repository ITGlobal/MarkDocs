using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ITGlobal.MarkDocs.Tags.Impl
{
    internal sealed class TagsExtensionStateBranch
    {
        public TagsExtensionStateBranch(IDocumentation documentation)
        {
            var tags = new HashSet<string>();
            var pageTags = new Dictionary<IPage, HashSet<string>>();
            var pageIndex = new Dictionary<string, HashSet<IPage>>();

            Add(pageTags, pageIndex, tags, documentation.RootPage);

            Tags = tags.ToArray();
            PageTags = pageTags.ToImmutableDictionary(_ => _.Key, _ => _.Value.ToArray());
            PageIndex = pageIndex.ToImmutableDictionary(_ => _.Key, _ => _.Value.ToArray());
        }

        public string[] Tags { get; }
        public ImmutableDictionary<IPage, string[]> PageTags { get; }
        public ImmutableDictionary<string, IPage[]> PageIndex { get; }

        private static void Add(
            Dictionary<IPage, HashSet<string>> pageTags,
            Dictionary<string, HashSet<IPage>> pageIndex,
            HashSet<string> tags,
            IPage page)
        {
            if (page.Metadata.Tags != null)
            {
                foreach (var tag in page.Metadata.Tags)
                {
                    Add(pageTags, pageIndex, tags, page, tag);
                }
            }

            foreach (var nestedPage in page.NestedPages)
            {
                Add(pageTags, pageIndex, tags, nestedPage);
            }
        }

        private static void Add(
            Dictionary<IPage, HashSet<string>> pageTags,
            Dictionary<string, HashSet<IPage>> pageIndex,
            HashSet<string> tags,
            IPage page,
            string tag)
        {
            tags.Add(tag);

            if (!pageTags.TryGetValue(page, out var list))
            {
                list = new HashSet<string>();
                pageTags.Add(page, list);
            }
            list.Add(tag);

            if (!pageIndex.TryGetValue(tag, out var pages))
            {
                pages = new HashSet<IPage>();
                pageIndex.Add(tag, pages);
            }
            pages.Add(page);


        }
    }
}