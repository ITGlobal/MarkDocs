using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class GeneratorResourceUrlResolver : IResourceUrlResolver
    {
        public string ResolveUrl(IResourceUrlResolutionContext context, IResourceId resource)
            => OutputCache.GetRelativeResourcePath(resource, context.Page);
    }
}