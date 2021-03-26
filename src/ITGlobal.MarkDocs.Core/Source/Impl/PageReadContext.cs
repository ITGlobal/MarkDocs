using System;
using System.IO;
using System.Linq;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Impl;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class PageReadContext : IPageReadContext, IResourceUrlResolutionContext
    {

        private readonly IShallowPageAssetReader _worker;
        private readonly ShallowPageAsset _asset;

        public PageReadContext(IShallowPageAssetReader worker, ShallowPageAsset asset)
        {
            _worker = worker;
            _asset = asset;

            var filename = Path.GetFileName(asset.AbsolutePath);
            IsIndexPage = worker.Format.IndexFileNames.Any(
                n => string.Equals(filename, n, StringComparison.OrdinalIgnoreCase)
            );
        }

        public string SourceTreeId => _worker.Root.SourceTree.Id;
        public IResourceId Page => _asset;
        public bool IsBranchPage => _asset is BranchShallowPageAsset;
        public bool IsIndexPage { get; }

        public bool TryResolvePageResource(string url, out string pageId, out string pageUrl)
        {
            var page = _worker.ResolvePageAsset(url);

            if (page == null)
            {
                pageId = null;
                pageUrl = null;
                return false;
            }

            pageId = page.Id;
            pageUrl = _worker.ResourceUrlResolver.ResolveUrl(this, page);

            return true;
        }

        public bool TryResolveFileResourcePath(string url, out string path)
        {
            var file = _worker.ResolveFileAsset(url) as PhysicalFileAsset;
            if (file == null)
            {
                path = null;
                return false;
            }

            path = file.AbsolutePath;
            return true;
        }

        public bool TryResolveFileResource(string url, out string fileId, out string fileUrl)
        {
            var file = _worker.ResolveFileAsset(url);

            if (file == null)
            {
                fileId = null;
                fileUrl = null;
                return false;
            }

            fileId = file.Id;
            fileUrl = _worker.ResourceUrlResolver.ResolveUrl(this, file);

            return true;
        }

        public void CreateAttachment(
            byte[] source,
            IGeneratedAssetContent content,
            out GeneratedFileAsset asset,
            out string url)
            => CreateAttachmentImpl(HashUtil.HashBuffer(source), content, out asset, out url);

        public void CreateAttachment(
            string source,
            IGeneratedAssetContent content,
            out GeneratedFileAsset asset,
            out string url)
            => CreateAttachmentImpl(HashUtil.HashString(source), content, out asset, out url);

        private void CreateAttachmentImpl(
            string hash,
            IGeneratedAssetContent content,
            out GeneratedFileAsset asset,
            out string url)
        {
            var filename = content.FormatFileName(hash);
            var id = $"/{filename}";
            asset = new GeneratedFileAsset(
                id: id,
                relativePath: filename,
                content: content,
                contentHash: hash
            );

            _worker.RegisterAsset(asset);
            url = _worker.ResourceUrlResolver.ResolveUrl(this, asset);
        }

        public void CreateAttachment(
            byte[] source,
            string filename,
            IGeneratedAssetContent content,
            out GeneratedFileAsset asset,
            out string url)
            => CreateAttachmentImpl(HashUtil.HashBuffer(source), content, out asset, out url);

        public void CreateAttachment(
            string source,
            string filename,
            IGeneratedAssetContent content,
            out GeneratedFileAsset asset,
            out string url)
            => CreateAttachmentImpl(HashUtil.HashString(source), content, out asset, out url);

        public void Warning(string message, int? lineNumber = null)
        {
            _worker.Report.Warning(_asset.AbsolutePath, message, lineNumber);
        }

        public void Error(string message, int? lineNumber = null)
        {
            _worker.Report.Error(_asset.AbsolutePath, message, lineNumber);
        }

        /// <summary>
        ///     Resolves content resource URL
        /// </summary>
        public ResolveResourceUrlResult ResolveResourceUrl(string url, int? lineNumber = null)
        {
            var originalUrl = url;

            if (url.Length > 0 && url[0] == '#')
            {
                return new ResolveResourceUrlResult(url, url.Substring(1));
            }

            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            {
                return ResolveResourceUrlResult.Empty;
            }

            if (uri.IsAbsoluteUri)
            {
                return ResolveResourceUrlResult.Empty;
            }

            string hash = null;
            var i = url.IndexOf('#');
            if (i >= 0)
            {
                hash = url.Substring(i + 1);
                url = url.Substring(0, i);
            }

            var isIndexFileLink = false;
            var filename = Path.GetFileName(url);
            foreach (var name in _worker.Format.IndexFileNames)
            {
                if (filename == name)
                {
                    url = Path.GetDirectoryName(url);
                    isIndexFileLink = true;
                    break;
                }
            }

            // Remove .md extension if specified
            if (!isIndexFileLink)
            {
                var ext = Path.GetExtension(url);
                if (_worker.Format.Extensions.Contains(ext))
                {
                    url = Path.ChangeExtension(url, null);
                }
            }

            try
            {
                if (!url.StartsWith("/") && !url.StartsWith("\\"))
                {
                    url = NormalizeResourceUrl(url);
                }
            }
            catch (InvalidOperationException)
            {
                Error($"URL \"{url}\" is not a valid relative reference", lineNumber);
                return ResolveResourceUrlResult.Empty;
            }

            if (TryResolvePageResource(url, out var pageId, out var pageUrl))
            {
                if (!string.IsNullOrEmpty(hash))
                {
                    if (pageId == Page.Id && originalUrl != $"#{hash}")
                    {
                        Warning($"Link ({url}#{hash}) can be replaced with (#{hash})", lineNumber);
                    }

                    pageUrl = $"{pageUrl}#{hash}";
                    return new ResolveResourceUrlResult(pageUrl, hash);
                }

                return new ResolveResourceUrlResult(pageUrl);
            }

            if (TryResolveFileResource(url, out _, out var fileUrl))
            {
                if (!string.IsNullOrEmpty(hash))
                {
                    Error($"URL \"{url}#{hash}\" points to a file but it has a hash", lineNumber);
                }

                return new ResolveResourceUrlResult(fileUrl);
            }

            Error($"Invalid hyperlink \"{url}\"", lineNumber);
            return ResolveResourceUrlResult.Empty;
        }

        /// <summary>
        ///     Normalizes resource URL
        /// </summary>
        public string NormalizeResourceUrl(string resourceUrl)
        {
            var normalizedUrl = NormalizeResourcePath(Page.Id, resourceUrl, IsIndexPage);
            return normalizedUrl;
        }

        private static string NormalizeResourcePath(string basePath, string resourceUrl, bool isIndexPage)
        {
            var basePathSegments = basePath.Split(new[] {'/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
            var basePathLen = basePathSegments.Length;

            if (!isIndexPage)
            {
                basePathLen--;
            }

            var resourcePathSegments = resourceUrl.Split(new[] {'/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
            var resourcePathLen = 0;
            for (var i = 0; i < resourcePathSegments.Length; i++)
            {
                if (resourcePathSegments[i] == "..")
                {
                    basePathLen--;
                    if (basePathLen < 0)
                    {
                        throw new InvalidOperationException("Invalid relative resource path");
                    }
                }
                else
                {
                    resourcePathLen++;
                }
            }

            var normalizedUrlSegments = new string[basePathLen + resourcePathLen];
            var index = 0;
            for (var i = 0; i < basePathLen; i++)
            {
                normalizedUrlSegments[index] = basePathSegments[i];
                index++;
            }

            for (var i = 0; i < resourcePathSegments.Length; i++)
            {
                if (resourcePathSegments[i] != "..")
                {
                    normalizedUrlSegments[index] = resourcePathSegments[i];
                    index++;
                }
            }

            var normalizedUrl = "/" + string.Join("/", normalizedUrlSegments);
            return normalizedUrl;
        }

    }
}