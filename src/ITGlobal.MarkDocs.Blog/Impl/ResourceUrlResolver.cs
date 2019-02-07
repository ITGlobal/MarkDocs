using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class ResourceUrlResolver : IResourceUrlResolver
    {
        private readonly string _rootUrl;

        public ResourceUrlResolver(string rootUrl)
        {
            while (rootUrl.EndsWith("/"))
            {
                rootUrl = rootUrl.Substring(0, rootUrl.Length - 1);
            }
            _rootUrl = rootUrl;
        }

        public string ResolveUrl(IResourceUrlResolutionContext context, IResourceId resource)
        {
            var url = _rootUrl + resource.Id;
            return url;
        }
    }
}