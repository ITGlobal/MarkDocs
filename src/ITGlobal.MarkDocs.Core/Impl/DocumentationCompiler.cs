using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class DocumentationCompiler
    {
        #region nested context class

        private sealed class PageContext : IPageRenderContext
        {
            private readonly DocumentationCompiler _compiler;

            public PageContext(DocumentationCompiler compiler, AssetTree assetTree, PageAsset page)
            {
                _compiler = compiler;
                AssetTree = assetTree;
                Page = page;
            }

            public PageAsset Page { get; }
            public AssetTree AssetTree { get; }

            public CreateAttachmentResult CreateAttachment(byte[] source, IGeneratedAssetContent content)
                => CreateAttachmentImpl(HashUtil.HashBuffer(source), content);

            public CreateAttachmentResult CreateAttachment(string source, IGeneratedAssetContent content)
                => CreateAttachmentImpl(HashUtil.HashString(source), content);

            private CreateAttachmentResult CreateAttachmentImpl(string hash, IGeneratedAssetContent content)
            {
                var filename = content.FormatFileName(hash);
                var id = $"/{filename}";
                var asset = new GeneratedAsset(
                    id: id,
                    relativePath: filename,
                    contentType: content.ContentType,
                    contentHash: hash
                );

                _compiler.Process(asset, content.Write);

                return new CreateAttachmentResult(asset, _compiler._transaction.Read);
            }

            public void Warning(string message, int? lineNumber = null, Exception exception = null)
                => _compiler._reportBuilder.ForFile(Page.RelativePath).Warning(message, lineNumber);

            public void Error(string message, int? lineNumber = null)
                => _compiler._reportBuilder.ForFile(Page.RelativePath).Error(message, lineNumber);

            public void Error(string message, Exception exception)
                => _compiler._reportBuilder.ForFile(Page.RelativePath).Error(message);
            // TODO write to log
        }

        #endregion

        #region fields

        private readonly ICacheUpdateTransaction _transaction;

        private readonly CompilationReportBuilder _reportBuilder = new CompilationReportBuilder();
        private readonly List<AttachmentModel> _attachments = new List<AttachmentModel>();

        #endregion

        #region .ctor

        public DocumentationCompiler(ICacheUpdateTransaction transaction)
        {
            _transaction = transaction;
        }

        #endregion

        #region public methods

        public DocumentationModel Compile(AssetTree assetTree)
        {
            var rootPage = Process(assetTree, assetTree.RootPage);

            foreach (var asset in assetTree.Attachments)
            {
                Process(asset);
            }

            var model = new DocumentationModel
            {
                Id = assetTree.Id,
                Info = new SourceInfoModel
                {
                    SourceUrl = assetTree.SourceInfo.SourceUrl,
                    Version = assetTree.SourceInfo.Version,
                    LastChangeId = assetTree.SourceInfo.LastChangeId,
                    LastChangeTime = assetTree.SourceInfo.LastChangeTime,
                    LastChangeAuthor = assetTree.SourceInfo.LastChangeAuthor,
                    LastChangeDescription = assetTree.SourceInfo.LastChangeDescription,
                },
                Attachments = _attachments.OrderBy(_ => _.Id).ToArray(),
                RootPage = rootPage,
                CompilationReport = _reportBuilder.Build(),

            };
            _transaction.Store(model);

            return model;
        }

        #endregion

        #region page rendering

        private PageModel Process(AssetTree assetTree, PageAsset asset)
        {
            var context = new PageContext(this, assetTree, asset);

            _transaction.Store(asset, stream => RenderPage(context, asset.Content, stream));

            var model = new PageModel
            {
                Id = asset.Id,
                Title = asset.Metadata.Title,
                Description = asset.Metadata.Description,
                FileName = asset.RelativePath,
                Metadata = asset.Metadata,
                Anchors = asset.Content.Anchors != null && asset.Content.Anchors.Count > 0
                    ? asset.Content.Anchors.Select(MapAnchor).ToArray()
                    : null
            };

            if (asset.Content.HasPreview)
            {
                var previewAsset = new PagePreviewAsset(
                    id: $"{asset.Id}__preview",
                    contentHash: asset.ContentHash
                );
                _transaction.Store(previewAsset, stream => RenderPagePreview(context, asset.Content, stream));

                model.Preview = new PagePreviewModel
                {
                    Id = previewAsset.Id,
                    FileName = asset.AbsolutePath,
                };
            }

            if (asset is BranchPageAsset branchPage)
            {
                var nestedPages = new List<PageModel>();
                foreach (var nestedPage in branchPage.Subpages)
                {
                    nestedPages.Add(Process(assetTree, nestedPage));
                }
                model.Pages = nestedPages.ToArray();
            }

            return model;
        }

        private static void RenderPage(PageContext context, IParsedPage page, Stream stream)
        {
            string html;
            try
            {
                html = page.Render(context);
            }
            catch (Exception e)
            {
                html = "<h1 style=\"color: red;\">Failed to render page</h1>";
                context.Error($"Failed to render page. {e.Message}", e);
            }

            using (var w = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                w.Write(html);
                w.Flush();
            }
        }

        private static void RenderPagePreview(PageContext context, IParsedPage page, Stream stream)
        {
            string html;
            try
            {
                html = page.RenderPreview(context);
            }
            catch (Exception e)
            {
                html = "<h1 style=\"color: red;\">Failed to render page preview</h1>";
                context.Error($"Failed to render page preview. {e.Message}", e);
            }

            using (var w = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                w.Write(html);
                w.Flush();
            }
        }

        private static PageAnchorModel MapAnchor(PageAnchor anchor)
        {
            return new PageAnchorModel
            {
                Id = anchor.Id,
                Title = anchor.Title,
                Nested = anchor.Nested != null && anchor.Nested.Length > 0
                    ? anchor.Nested.Select(MapAnchor).ToArray()
                    : null
            };
        }

        #endregion

        #region attachment rendering

        private void Process(AttachmentAsset asset)
        {
            var model = new AttachmentModel
            {
                Id = asset.Id,
                FileName = asset.AbsolutePath,
                Type = AttachmentType.File,
                ContentType = asset.ContentType,
            };

            _transaction.Store(asset, stream => RenderAttachment(asset, stream));
            _attachments.Add(model);
        }

        private void RenderAttachment(AttachmentAsset asset, Stream stream)
        {
            using (var source = File.OpenRead(asset.AbsolutePath))
            {
                source.CopyTo(stream);
            }
        }

        private void Process(GeneratedAsset asset, Action<Stream> write)
        {
            var model = new AttachmentModel
            {
                Id = asset.Id,
                FileName = asset.RelativePath,
                Type = AttachmentType.Generated,
                ContentType = asset.ContentType,
            };

            _transaction.Store(asset, write);
            _attachments.Add(model);
        }

        #endregion
    }
}