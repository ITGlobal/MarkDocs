namespace ITGlobal.MarkDocs.Example.Model
{
    public sealed class PageTreeNodeModel
    {
        public PageTreeNodeModel(IPage page, IPage selectedPage)
        {
            Page = page;
            SelectedPage = selectedPage;
        }

        public IPage Page { get; }
        public IPage SelectedPage { get; }
        public IDocumentation Documentation => SelectedPage.Documentation;
    }
}