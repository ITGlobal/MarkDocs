using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.StaticGen
{
    internal sealed class ResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IResource resource, IResource relativeTo) 
            => OutputCache.GetRelativeResourcePath(resource, relativeTo);
    }
}