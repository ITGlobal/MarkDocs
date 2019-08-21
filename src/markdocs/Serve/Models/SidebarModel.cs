namespace ITGlobal.MarkDocs.Tools.Serve.Models
{
    public sealed class SidebarModel
    {
        public SidebarModel(IDocumentation documentation)
        {
            RootPage=documentation.RootPage;
            ActivePage = null;
        }

        public SidebarModel(IPage page)
        {
            RootPage = page.Documentation.RootPage;
            ActivePage = page;
        }

        public IPage RootPage { get; }
        public IPage ActivePage { get; }
    }
}