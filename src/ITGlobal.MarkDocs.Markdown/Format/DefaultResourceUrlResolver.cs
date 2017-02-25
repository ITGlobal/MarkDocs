namespace ITGlobal.MarkDocs.Format
{
    internal sealed class DefaultResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IResource resource, IResource relativeTo)
        {
            return $"/{resource.Id}";;
        }
    }
}