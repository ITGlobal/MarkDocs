namespace ITGlobal.MarkDocs.Format.Impl
{
    internal sealed class DefaultResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IPageRenderContext context, IResourceId resource)
        {
            return $"/{resource.Id}"; ;
        }
    }
}