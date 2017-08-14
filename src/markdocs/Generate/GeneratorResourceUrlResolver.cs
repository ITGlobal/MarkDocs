using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class GeneratorResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IResource resource, IResource relativeTo)
            => OutputCache.GetRelativeResourcePath(resource, relativeTo);
    }
}