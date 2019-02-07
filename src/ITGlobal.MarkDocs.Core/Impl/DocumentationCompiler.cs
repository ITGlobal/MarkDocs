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
            
            public CreateAttachmentResult Store(GeneratedFileAsset asset)
            {
                _compiler.Process(asset);

                return new CreateAttachmentResult(asset, _compiler._transaction.Read);
            }
            
            public void Warning(string message, int? lineNumber = null, Exception exception = null)
                => _compiler._reportBuilder.Warning(Page.AbsolutePath, message, lineNumber);

            public void Error(string message, int? lineNumber = null)
                => _compiler._reportBuilder.Error(Page.AbsolutePath, message, lineNumber);

            public void Error(string message, Exception exception)
            {
                // todo write exception to log
                _compiler._reportBuilder.Error(Page.AbsolutePath, message);
            }
        }

        #endregion

        #region fields

        private readonly ICacheUpdateTransaction _transaction;

        private readonly CompilationReportBuilder _reportBuilder;
        private readonly Dictionary<string, FileModel> _attachments
            = new Dictionary<string, FileModel>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region .ctor

        public DocumentationCompiler(ICacheUpdateTransaction transaction, CompilationReportBuilder reportBuilder)
        {
            _transaction = transaction;
            _reportBuilder = reportBuilder;
        }

        #endregion

        #region public methods

        public DocumentationModel Compile(AssetTree assetTree)
        {
            var rootPage = Process(assetTree, assetTree.RootPage);

            foreach (var asset in assetTree.Files)
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
                Files = _attachments.OrderBy(_ => _.Key).Select(_=>_.Value).ToArray(),
                RootPage = rootPage,
                CompilationReport = _reportBuilder.Build(assetTree.RootDirectory),
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
                RelativePath = asset.RelativePath,
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
                    RelativePath = asset.RelativePath,
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

        private static void RenderPage(PageContext context, IPageContent pageContent, Stream stream)
        {
            string html;
            try
            {
                html = pageContent.Render(context);
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

        private static void RenderPagePreview(PageContext context, IPageContent pageContent, Stream stream)
        {
            string html;
            try
            {
                html = pageContent.RenderPreview(context);
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

        private void Process(FileAsset asset)
        {
            if (_attachments.ContainsKey(asset.Id))
            {
                return;
            }

            switch (asset)
            {
                case PhysicalFileAsset a:
                    {
                        var model = new FileModel
                        {
                            Id = a.Id,
                            RelativePath = a.RelativePath,
                            Type = AttachmentType.File,
                            ContentType = a.ContentType,
                        };

                        _transaction.Store(a, stream => RenderAttachment(a, stream));
                        _attachments.Add(asset.Id, model);
                    }

                    break;

                case GeneratedFileAsset a:
                    {
                        var model = new FileModel
                        {
                            Id = a.Id,
                            RelativePath = a.RelativePath,
                            Type = AttachmentType.Generated,
                            ContentType = a.ContentType,
                        };

                        _transaction.Store(a, a.Content.Write);
                        _attachments.Add(asset.Id, model);
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unknown asset file: \"{asset?.GetType()?.Name}\"");
               
            }
        }

        private void RenderAttachment(PhysicalFileAsset asset, Stream stream)
        {
            using (var source = File.OpenRead(asset.AbsolutePath))
            {
                source.CopyTo(stream);
            }
        }

        private void Process(GeneratedFileAsset asset, Action<Stream> write)
        {
            var model = new FileModel
            {
                Id = asset.Id,
                RelativePath = asset.RelativePath,
                Type = AttachmentType.Generated,
                ContentType = asset.ContentType,
            };

            _transaction.Store(asset, write);
            _attachments.Add(asset.Id, model);
        }

        #endregion
    }
}