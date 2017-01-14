namespace ITGlobal.MarkDocs.Example.Model
{
    public sealed class PageTreeModel
    {
        public PageTreeModel(IPageTree tree, IPage page)
        {
            Tree = tree;
            Page = page;
        }

        public IPageTree Tree { get; }
        public IPage Page { get; }
        public IDocumentation Documentation => Page.Documentation;
    }
}