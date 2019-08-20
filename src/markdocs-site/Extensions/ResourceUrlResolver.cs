using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Site.Extensions
{
    public sealed class ResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IResourceUrlResolutionContext context, IResourceId resource)
        {
            return resource.GetResourceUrl();
        }
    }
}
