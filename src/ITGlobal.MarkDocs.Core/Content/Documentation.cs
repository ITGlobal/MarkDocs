using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs.Cache;
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
        private readonly RootDirectoryPageTreeNode _pageTree;

        private readonly Dictionary<string, Page> _pages = new Dictionary<string, Page>(StringComparer.OrdinalIgnoreCase);
        private readonly List<IAttachment> _attachments = new List<IAttachment>();
        private readonly Dictionary<string, Attachment> _attachmentsById = new Dictionary<string, Attachment>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public Documentation(
            MarkDocService service,
            string id,
            IContentVersion contentVersion,
            RootDirectoryPageTreeNode pageTree)
        {
            NormalizeId(ref id);

            Id = id;
            _service = service;
            _pageTree = pageTree;
            ContentVersion = contentVersion;

            // Build page dictionary
            pageTree.ScanNodes((node, _) =>
            {
                if (node.IsHyperlink)
                {
                    var page = new Page(this, _service.Format, _service.Cache, node);
                    _pages[node.Id] = page;
                }
            });

            // Link nodes to documentation
            pageTree.LinkToDocumentation(this);

            // Build attachment dictionary
            foreach (var path in pageTree.Attachments)
            {
                var relativePath = DirectoryScanner.GetRelativePath(pageTree.RootDirectory, path);

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
        public IContentVersion ContentVersion { get; }

        /// <summary>
        ///     Page tree
        /// </summary>
        public IPageTree PageTree => _pageTree;

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

            Page page;
            if (!_pages.TryGetValue(id, out page))
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

            Attachment attachment;
            if (!_attachmentsById.TryGetValue(id, out attachment))
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
        public void Compile(ICacheUpdateOperation operation)
        {
            using (_service.Log.BeginScope("Compile({0})", Id))
            {
                // Clear cache
                operation.Clear(this);

                // Compile pages and put them into cache
                var i = 0;
                foreach (var page in _pages.Values)
                {
                    i++;
                    _service.Callback.CompilingPage(Id, page.Id, i, _pages.Count);

                    page.Compile(operation);
                    _service.Log.LogDebug("Compiled page {0}:{1}", Id, page.RelativeFileName);
                }

                // Put each non-page file into cache
                i = 0;
                foreach (var attachment in _attachmentsById.Values)
                {
                    i++;
                    _service.Callback.CachingAttachment(Id, attachment.Id, i, _pages.Count);

                    attachment.PutIntoCache(operation);
                    _service.Log.LogDebug("Cached file {0}:{1}", Id, attachment.FileName);
                }
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