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

        public void CreateAttachment(byte[] source, IGeneratedAssetContent content, out GeneratedFileAsset asset, out string url)
            => CreateAttachmentImpl(HashUtil.HashBuffer(source), content, out asset, out url);

        public void CreateAttachment(string source, IGeneratedAssetContent content, out GeneratedFileAsset asset, out string url)
            => CreateAttachmentImpl(HashUtil.HashString(source), content, out asset, out url);

        private void CreateAttachmentImpl(string hash, IGeneratedAssetContent content, out GeneratedFileAsset asset, out string url)
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

        public void CreateAttachment(byte[] source, string filename, IGeneratedAssetContent content, out GeneratedFileAsset asset, out string url)
            => CreateAttachmentImpl(HashUtil.HashBuffer(source), content, out asset, out url);

        public void CreateAttachment(string source, string filename, IGeneratedAssetContent content, out GeneratedFileAsset asset, out string url)
            => CreateAttachmentImpl(HashUtil.HashString(source), content, out asset, out url);

        private void CreateAttachmentImpl(string hash, string filename, IGeneratedAssetContent content, out GeneratedFileAsset asset, out string url)
        {
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

        public void Warning(string message, int? lineNumber = null)
        {
            _worker.Report.Warning(_asset.AbsolutePath, message, lineNumber);
        }

        public void Error(string message, int? lineNumber = null)
        {
            _worker.Report.Error(_asset.AbsolutePath, message, lineNumber);
        }
    }
}