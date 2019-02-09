using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Source;
using System;
using System.IO;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class OutputCache : ICacheProvider
    {
        private readonly string _directory;
        private readonly ITemplate _template;

        public OutputCache(string directory, ITemplate template)
        {
            _directory = directory;
            _template = template;
        }

        public CacheDocumentationModel[] Load() => Array.Empty<CacheDocumentationModel>();

        public ICacheUpdateTransaction BeginTransaction(
            ISourceTree sourceTree, 
            ISourceInfo sourceInfo,
            CompilationEventListener listener, 
            bool forceCacheClear = false)
        {
            return new OutputCacheUpdateTransaction(_directory, _template, listener);
        }

        public void Drop(string documentationId) { }

        public static string GetRelativeResourcePath(IResourceId resource, IResourceId relativeTo)
        {
            var relativeToUrl = new Uri($"http://site{GetResourcePath(relativeTo)}");
            var resourceUrl = new Uri($"http://site{GetResourcePath(resource)}");

            var url = relativeToUrl.MakeRelativeUri(resourceUrl);
            var urlstr = url.ToString();

            if (urlstr.StartsWith("/"))
            {
                urlstr = urlstr.Substring(1);
            }

            if (string.IsNullOrEmpty(urlstr))
            {
                urlstr = Path.GetFileName(relativeToUrl.LocalPath);
            }

            return urlstr;
        }

        public static string GetResourcePath(IResourceId resource)
        {
            var path = resource.Id;
            if (path == "/")
            {
                path = "/index";
            }

            if (resource.Type == ResourceType.Page)
            {
                path = Path.ChangeExtension(path, ".html");
            }

            return path;
        }
    }
}