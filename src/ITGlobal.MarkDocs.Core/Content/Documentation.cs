using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Storage;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     A documentation
    /// </summary>
    internal sealed class Documentation : IDocumentation
    {
        #region fields

        private readonly MarkDocService _service;
        private readonly IContentDirectory _contentDirectory;
        private readonly ICompilationReportBuilder _compilationReport = new CompilationReportBuilder();

        private readonly Dictionary<string, Page> _pages = new Dictionary<string, Page>(StringComparer.OrdinalIgnoreCase);
        private readonly List<IAttachment> _attachments = new List<IAttachment>();
        private readonly Dictionary<string, Attachment> _attachmentsById = new Dictionary<string, Attachment>(StringComparer.OrdinalIgnoreCase);

        private RootDirectoryPageTreeNode _pageTree;


        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public Documentation(MarkDocService service, IContentDirectory contentDirectory)
        {
            var id = contentDirectory.Id;
            NormalizeId(ref id);

            Id = id;
            _service = service;
            _contentDirectory = contentDirectory;
        }

        #endregion

        #region IDocumentation

        /// <summary>
        ///     Documentation service
        /// </summary>
        public IMarkDocService Service => _service;

        /// <summary>
        ///     Documentation ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Documentation title
        /// </summary>
        public string Title => _pageTree.Title;

        /// <summary>
        ///     Documentation version
        /// </summary>
        public IContentVersion ContentVersion => _contentDirectory.ContentVersion;

        /// <summary>
        ///     Page tree
        /// </summary>
        public IPageTree PageTree => _pageTree;

        /// <summary>
        ///     Provides errors and warning for documentation
        /// </summary>
        public ICompilationReport CompilationReport { get; private set; }

        /// <summary>
        ///     Documentation attached files
        /// </summary>
        public IReadOnlyList<IAttachment> Attachments => _attachments;

        /// <summary>
        ///     Gets a documentation page by its ID
        /// </summary>
        /// <param name="id">
        ///     Page ID
        /// </param>
        /// <returns>
        ///     A documentation page or null if page doesn't exist
        /// </returns>
        public IPage GetPage(string id)
        {
            ResourceId.Normalize(ref id);

            if (!_pages.TryGetValue(id, out var page))
            {
                return null;
            }

            return page;
        }

        /// <summary>
        ///     Gets an attachment by its ID
        /// </summary>
        /// <param name="id">
        ///     Attachment ID
        /// </param>
        /// <returns>
        ///     An attachment or null if it doesn't exist
        /// </returns>
        public IAttachment GetAttachment(string id)
        {
            ResourceId.Normalize(ref id);

            if (!_attachmentsById.TryGetValue(id, out var attachment))
            {
                return null;
            }

            return attachment;
        }

        #endregion

        #region methods

        /// <summary>
        ///     Compiles documentation and builds cached pages
        /// </summary>
        public void Compile(DirectoryScanner directoryScanner, ICacheUpdateOperation operation)
        {
            using (_service.Log.BeginScope("Compile({0})", Id))
            {
                // Scan content directory and populate pages
                ScanDirectoryTree(directoryScanner);

                // Clear cache
                operation.Clear(this);

                // Parse pages
                ParsePages();

                // Render pages and put them into cache
                RenderPages(operation);

                // Wait for pending page compilations since they might produce new attachments
                operation.Flush();

                // Put each non-page file into cache
                CacheAttachments(operation);

                CompilationReport = _compilationReport.Build();
            }
        }

        private void ScanDirectoryTree(DirectoryScanner directoryScanner)
        {
            // Scan content directory
            _pageTree = directoryScanner.ScanDirectory(_contentDirectory.Path, _compilationReport);

            // Populate pages
            _pageTree.ScanNodes((node, _) =>
            {
                if (node.IsHyperlink)
                {
                    var page = new Page(this, _service.Format, _service.Cache, node);
                    _pages[node.Id] = page;
                }
            });

            // Link nodes to documentation
            _pageTree.LinkToDocumentation(this);

            // Populate attachments
            foreach (var path in _pageTree.Attachments)
            {
                var relativePath = DirectoryScanner.GetRelativePath(_pageTree.RootDirectory, path);

                string contentType;
                if (!_service.ContentTypeProvider.TryGetContentType(path, out contentType))
                {
                    contentType = Attachment.DEFAULT_MIME_TYPE;
                }

                var attachment = new FileAttachment(this, _service.Cache, relativePath, path, contentType);
                _attachmentsById[attachment.Id] = attachment;
                _attachments.Add(attachment);
            }
        }

        private void ParsePages()
        {
            var i = 0;
            foreach (var page in _pages.Values)
            {
                i++;
                _service.Callback.ParsedPage(Id, page.Id, i, _pages.Count);

                page.Parse(_compilationReport.ForPage(page));
                _service.Log.LogDebug("Parsed page {0}:{1}", Id, page.RelativeFilePath);
            }
        }

        private void RenderPages(ICacheUpdateOperation operation)
        {
            var i = 0;
            foreach (var page in _pages.Values)
            {
                i++;

                var j = i;
                var p = page;

                page.Render(operation, _compilationReport.ForPage(page), () =>
                {
                    _service.Callback.RenderedPage(Id, p.Id, j, _pages.Count);
                });
            }
        }

        private void CacheAttachments(ICacheUpdateOperation operation)
        {
            var i = 0;
            foreach (var attachment in _attachmentsById.Values)
            {
                i++;

                var j = i;
                var a = attachment;

                attachment.PutIntoCache(operation, () =>
                {
                    _service.Callback.CachedAttachment(Id, a.Id, j, _attachmentsById.Count);
                });
            }
        }

        /// <summary>
        ///   Add a generated attachment
        /// </summary>
        public IAttachment CreateAttachment(string name, byte[] content)
        {
            if (name == null)
            {
                name = Guid.NewGuid().ToString("N");
            }
            name = name.ToLowerInvariant();

            var id = "/" + name;
            while (_attachmentsById.ContainsKey(name))
            {
                id = $"/{name}_{Guid.NewGuid():N}";
            }

            string contentType;
            if (!_service.ContentTypeProvider.TryGetContentType(name, out contentType))
            {
                contentType = Attachment.DEFAULT_MIME_TYPE;
            }

            var attachment = new GeneratedAttachment(
                this,
                _service.Cache,
                id,
                name,
                contentType,
                content);

            _attachmentsById[attachment.Id] = attachment;
            _attachments.Add(attachment);

            return attachment;
        }

        /// <summary>
        ///     Normalizes a documentation ID
        /// </summary>
        public static void NormalizeId(ref string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            id = id.ToLowerInvariant();
        }

        #endregion
    }
}