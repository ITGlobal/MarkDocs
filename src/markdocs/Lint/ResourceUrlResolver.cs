using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public sealed class ResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IResourceUrlResolutionContext context, IResourceId resource)
            => Linter.GetRelativeResourcePath(resource, context.Page);
    }
}