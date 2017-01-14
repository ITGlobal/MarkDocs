namespace ITGlobal.MarkDocs.Example.Model
{
    public sealed class PageTreeNodeModel
    {
        public PageTreeNodeModel(IPageTreeNode node, IPage page)
        {
            Node = node;
            Page = page;
        }

        public IPageTreeNode Node { get; }
        public IPage Page { get; }
        public IDocumentation Documentation => Page.Documentation;
    }
}