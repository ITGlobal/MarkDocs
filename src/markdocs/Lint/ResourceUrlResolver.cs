using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class ResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IResource resource, IResource relativeTo)
            => Linter.GetRelativeResourcePath(resource, relativeTo);
    }
}