using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Example.Model
{
    public sealed class PagesByTagModel
    {
        public PagesByTagModel(IDocumentation documentation, string tag, IReadOnlyList<IPage> pages)
        {
            Documentation = documentation;
            Tag = tag;
            Pages = pages;
        }

        public IDocumentation Documentation { get; }
        public string Tag{ get; }
        public IReadOnlyList<IPage> Pages { get; }
    }
}