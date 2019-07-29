using ITGlobal.MarkDocs.Format.Impl.Extensions.Cut;
using ITGlobal.MarkDocs.Format.Impl.Metadata;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ITGlobal.MarkDocs.Format.Impl
{
    /// <summary>
    ///     A parsed markdown page
    /// </summary>
    internal sealed class MarkdownPageContent : IPageContent
    {
        #region nested types

        private readonly struct ValidateAnchorQueueItem
        {
            public ValidateAnchorQueueItem(string pageId, string hash, LinkInline link)
            {
                PageId = pageId;
                Hash = hash;
                Link = link;
            }

            public readonly string PageId;
            public readonly string Hash;
            public readonly LinkInline Link;

            public void Deconstruct(out string pageId, out string hash, out LinkInline link)
            {
                pageId = PageId;
                hash = Hash;
                link = Link;
            }
        }

        #endregion

        #region fields

        private readonly MarkdownDocument _ast;
        private readonly MarkdownPipeline _pipeline;
        private readonly Queue<ValidateAnchorQueueItem> _validateAnchorsQueue;
        private readonly int? _cutBlockIndex;

        #endregion

        #region .ctor

        private MarkdownPageContent(
            MarkdownDocument ast,
            MarkdownPipeline pipeline,
            Queue<ValidateAnchorQueueItem> validateAnchorsQueue,
            PageMetadata metadata,
            PageAnchors anchors,
            int? cutBlockIndex)
        {
            _ast = ast;
            _pipeline = pipeline;
            _validateAnchorsQueue = validateAnchorsQueue;
            Metadata = metadata;
            Anchors = anchors;
            _cutBlockIndex = cutBlockIndex;
        }

        #endregion

        #region factory

        public static MarkdownPageContent Read(
            IPageReadContext ctx,
            IMetadataExtractor metadataExtractor,
            MarkdownPipeline pipeline,
            string markup)
        {
            using (MarkdownPageReadContext.Use(ctx))
            {
                // Parse AST
                var ast = Markdown.Parse(markup, pipeline);

                // Resolve and rewrite URL in links and images
                RewriteLinkUrls(ctx, ast, out var validateAnchorsQueue);

                // Extract metadata from AST
                var metadata = metadataExtractor.Extract(ctx, ast);
                var anchors = new PageAnchors(PageAnchorReader.Read(ast));

                // Try generate a preview AST
                var (cutBlock, cutBlockIndex) = ast
                    .Select((b, i) => (block: b, index: i))
                    .FirstOrDefault(_ => _.block is CutBlock);

                return new MarkdownPageContent(
                    ast,
                    pipeline,
                    validateAnchorsQueue,
                    metadata,
                    anchors,
                    cutBlock != null ? cutBlockIndex : (int?)null
                );
            }
        }

        private static MarkdownDocument GetPreviewAst(MarkdownDocument ast, MarkdownPipeline pipeline, string markup)
        {
            var (block, index) = ast
                .Select((b, i) => (block: b, index: i))
                .FirstOrDefault(_ => _.block is CutBlock);
            if (block == null)
            {
                return null;
            }

            var previewAst = Markdown.Parse(markup, pipeline);
            while (previewAst.Count > index)
            {
                previewAst.RemoveAt(previewAst.Count - 1);
            }

            return previewAst;
        }

        #endregion

        #region properties

        public PageMetadata Metadata { get; }

        public PageAnchors Anchors { get; }

        /// <summary>
        ///     true if page contains a "cut" separator
        /// </summary>
        public bool HasPreview => _cutBlockIndex != null;

        #endregion

        #region url resolution

        private static void RewriteLinkUrls(IPageReadContext ctx, MarkdownDocument ast, out Queue<ValidateAnchorQueueItem> validateAnchorsQueue)
        {
            validateAnchorsQueue = new Queue<ValidateAnchorQueueItem>();
            foreach (var link in ast.Descendants().OfType<LinkInline>())
            {
                if (link.IsRewrittenLink())
                {
                    continue;
                }

                var url = ResolveLinkUrl(ctx, link, validateAnchorsQueue);
                if (url != null)
                {
                    link.Url = url;
                    link.SetIsRewrittenLink();
                }
            }
        }

        [CanBeNull]
        private static string ResolveLinkUrl(IPageReadContext ctx, LinkInline link, Queue<ValidateAnchorQueueItem> validateAnchorsQueue)
        {
            var url = link.Url;
            if (url.Length > 0 && url[0] == '#')
            {
                validateAnchorsQueue.Enqueue(new ValidateAnchorQueueItem(ctx.Page.Id, url.Substring(1), link));
                return url;
            }

            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            {
                return null;
            }

            if (uri.IsAbsoluteUri)
            {
                return null;
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
            foreach (var name in MarkdownFormat.IndexFileNames)
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
                if (MarkdownFormat.Extensions.Contains(ext))
                {
                    url = Path.ChangeExtension(url, null);
                }
            }

            try
            {
                if (!url.StartsWith("/") && !url.StartsWith("\\"))
                {
                    url = NormalizeResourcePath(ctx, url);
                }
            }
            catch (InvalidOperationException)
            {
                ctx.Error($"URL \"{url}\" is not a valid relative reference", link.Line);
                return null;
            }

            if (ctx.TryResolvePageResource(url, out var pageId, out var pageUrl))
            {
                if (!string.IsNullOrEmpty(hash))
                {
                    validateAnchorsQueue.Enqueue(new ValidateAnchorQueueItem(pageId, hash, link));

                    if (pageId == ctx.Page.Id && link.Url != $"#{hash}")
                    {
                        var text = link.GetText();
                        ctx.Warning(
                            $"Link [{text}]({url}#{hash}) can be replaced with [{text}](#{hash})",
                            link.Line
                        );
                    }

                    pageUrl = $"{pageUrl}#{hash}";
                    return pageUrl;
                }

                return pageUrl;
            }

            if (ctx.TryResolveFileResource(url, out _, out var fileUrl))
            {
                if (!string.IsNullOrEmpty(hash))
                {
                    ctx.Error(
                        $"URL \"{url}#{link}\" points to a file but it has a hash",
                        link.Line
                    );
                }

                return fileUrl;
            }

            ctx.Error($"Invalid hyperlink \"{url}\"", link.Line);
            return null;
        }

        private static string NormalizeResourcePath(IPageReadContext ctx, string resourceUrl)
        {
            var normalizedUrl = NormalizeResourcePath(ctx.Page.Id, resourceUrl, ctx.IsBranchPage || ctx.IsIndexPage);
            return normalizedUrl;
        }

        private static string NormalizeResourcePath(string basePath, string resourceUrl, bool isIndexPage)
        {
            var basePathSegments = basePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var basePathLen = basePathSegments.Length;

            if (!isIndexPage)
            {
                basePathLen--;
            }

            var resourcePathSegments = resourceUrl.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
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

        #endregion

        #region rendering

        /// <summary>
        ///     Renders page into HTML
        /// </summary>
        public string Render(IPageRenderContext ctx)
        {
            using (MarkdownPageRenderContext.Use(ctx))
            {
                // Validate anchor links links
                while (_validateAnchorsQueue.Count > 0)
                {
                    var (pageId, hash, link) = _validateAnchorsQueue.Dequeue();
                    ValidateAnchorLink(ctx, link, pageId, hash);
                }

                // Render HTML
                string html;
                using (var writer = new StringWriter())
                {
                    var renderer = new HtmlRenderer(writer);
                    _pipeline.Setup(renderer);
                    renderer.Render(_ast);
                    writer.Flush();

                    html = writer.ToString();
                }

                return html;
            }
        }

        /// <summary>
        ///     Renders page preview into HTML
        /// </summary>
        public string RenderPreview(IPageRenderContext ctx)
        {
            if (_cutBlockIndex == null)
            {
                throw new Exception("This page doesn't have a preview");
            }

            using (MarkdownPageRenderContext.Use(ctx))
            {
                // Validate anchor links links
                while (_validateAnchorsQueue.Count > 0)
                {
                    var (pageId, hash, link) = _validateAnchorsQueue.Dequeue();
                    ValidateAnchorLink(ctx, link, pageId, hash);
                }

                // Render HTML
                string html;
                using (var writer = new StringWriter())
                {
                    var renderer = new HtmlRenderer(writer);
                    _pipeline.Setup(renderer);
                    for (var i = 0; i < _cutBlockIndex.Value; i++)
                    {
                        renderer.Render(_ast[i]);
                    }
                    writer.Flush();

                    html = writer.ToString();
                }

                return html;
            }
        }

        private string Render(IPageRenderContext ctx, MarkdownDocument ast)
        {
            using (MarkdownPageRenderContext.Use(ctx))
            {
                // Validate anchor links links
                while (_validateAnchorsQueue.Count > 0)
                {
                    var (pageId, hash, link) = _validateAnchorsQueue.Dequeue();
                    ValidateAnchorLink(ctx, link, pageId, hash);
                }

                // Render HTML
                string html;
                using (var writer = new StringWriter())
                {
                    var renderer = new HtmlRenderer(writer);
                    _pipeline.Setup(renderer);
                    renderer.Render(ast);
                    writer.Flush();

                    html = writer.ToString();
                }

                return html;
            }
        }

        private void ValidateAnchorLink(IPageRenderContext ctx, LinkInline link, string url, string hash)
        {
            var page = ctx.AssetTree.TryGetAsset(url) as PageAsset;
            if (page == null)
            {
                ctx.Error($"Invalid hyperlink \"{link.Url}\"", link.Line);
                return;
            }

            if (page.Content.Anchors == null ||
                page.Content.Anchors[hash] == null)
            {
                ctx.Error(
                    $"Anchor #{hash} doesn't exist on page \"{page.RelativePath}\"",
                    link.Line
                );
            }
            else if (page.Id == ctx.Page.Id && link.Url != $"#{hash}")
            {
                var text = link.GetText();
                ctx.Warning(
                    $"Link [{text}]({url}#{hash}) can be replaced with [{text}](#{hash})",
                    link.Line
                );
            }
        }

        #endregion
    }
}
