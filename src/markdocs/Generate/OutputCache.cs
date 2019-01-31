using System;
using System.Collections.Generic;
using System.IO;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Storage;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class OutputCache : ICache
    {
        private readonly string _directory;
        private readonly ITemplate _template;

        public OutputCache(string directory, ITemplate template)
        {
            _directory = directory;
            _template = template;
        }

        public CacheVerifyResult Verify(IReadOnlyList<IContentDirectory> contentDirectories)
        {
            return CacheVerifyResult.OutOfDate;
        }

        public ICacheUpdateOperation_OLD BeginUpdate()
            => new OutputCacheUpdateOperation(_directory, _template);

        public Stream Read(IResource item)
        {
            throw new NotSupportedException();
        }

        public static string GetRelativeResourcePath(IResource resource, IResource relativeTo)
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

        public static string GetResourcePath(IResource resource)
        {
            var path = resource.Id;
            if (path == "/")
            {
                path = "/index";
            }

            string extension;
            switch (resource.Type)
            {
                case ResourceType.Page:
                    extension = ".html";
                    break;
                default:
                    extension = Path.GetExtension(resource.FileName);
                    break;
            }

            path = Path.ChangeExtension(path, extension);
            return path;
        }
    }
}