using System.Collections.Generic;
using System.Linq;

namespace ITGlobal.MarkDocs.Tags
{
    internal sealed class TagsExtensionStateBranch
    {
        public TagsExtensionStateBranch(IDocumentation documentation)
        {
            var tags = new HashSet<string>();
            var pageTags = new Dictionary<IPage, HashSet<string>>();
            var pageIndex = new Dictionary<string, HashSet<IPage>>();

            foreach (var node in documentation.PageTree.Pages)
            {
                var page = documentation.GetPage(node.Id);
                if (page != null)
                {
                    foreach (var tag in page.Metadata.Tags)
                    {
                        var t = TagsExtension.NormalizeTag(tag);
                        tags.Add(t);
                        Add(pageTags, pageIndex, page, t);
                    }
                }
            }

            Tags = tags.ToArray();
            PageTags = pageTags.ToDictionary(_ => _.Key, _ => _.Value.ToArray());
            PageIndex = pageIndex.ToDictionary(_ => _.Key, _ => _.Value.ToArray());
        }

        public string[] Tags { get; }
        public Dictionary<IPage, string[]> PageTags { get; }
        public Dictionary<string, IPage[]> PageIndex { get; }

        private static void Add(
            Dictionary<IPage, HashSet<string>> pageTags,
            Dictionary<string, HashSet<IPage>> pageIndex,
            IPage page,
            string tag)
        {
            HashSet<string> tags;
            if (!pageTags.TryGetValue(page, out tags))
            {
                tags = new HashSet<string>();
                pageTags.Add(page, tags);
            }

            tags.Add(tag);


            HashSet<IPage> pages;
            if (!pageIndex.TryGetValue(tag, out pages))
            {
                pages = new HashSet<IPage>();
                pageIndex.Add(tag, pages);
            }

            pages.Add(page);
        }
    }
}