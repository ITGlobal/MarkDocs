using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Tools.Serve.Extensions
{
    public sealed class ResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IResourceUrlResolutionContext context, IResourceId resource)
        {
            return resource.GetResourceUrl();
        }
    }
}
