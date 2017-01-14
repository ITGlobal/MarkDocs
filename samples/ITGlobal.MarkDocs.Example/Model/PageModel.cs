namespace ITGlobal.MarkDocs.Example.Model
{
    public sealed class PageModel
    {
        public PageModel(IDocumentation documentation, IPage page)
        {
            Documentation = documentation;
            Page = page;
        }

        public IDocumentation Documentation { get; }
        public IPage Page{ get; }
    }
}