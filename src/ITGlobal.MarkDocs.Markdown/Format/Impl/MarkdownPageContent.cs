using ITGlobal.MarkDocs.Format.Impl.Extensions.Cut;
using ITGlobal.MarkDocs.Format.Impl.Metadata;
using ITGlobal.MarkDocs.Source;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    cutBlock != null ? cutBlockIndex : (int?) null
                );
            }
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

        private static void RewriteLinkUrls(
            IPageReadContext ctx,
            MarkdownDocument ast,
            out Queue<ValidateAnchorQueueItem> validateAnchorsQueue)
        {
            validateAnchorsQueue = new Queue<ValidateAnchorQueueItem>();
            foreach (var link in ast.Descendants().OfType<LinkInline>())
            {
                if (link.IsRewrittenLink())
                {
                    continue;
                }

                var (url, anchor) = ctx.ResolveResourceUrl(link.Url, link.Line);
                if (url != null)
                {
                    link.Url = url;
                    link.SetIsRewrittenLink();
                }

                if (anchor != null)
                {
                    validateAnchorsQueue.Enqueue(new ValidateAnchorQueueItem(ctx.Page.Id, anchor, link));
                }
            }
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

        /// <summary>
        ///     Runs page content validation
        /// </summary>
        public void Validate(IPageValidateContext ctx, AssetTree assetTree)
        {
            while (_validateAnchorsQueue.Count > 0)
            {
                var (pageId, hash, link) = _validateAnchorsQueue.Dequeue();
                ValidateAnchorLink(ctx, assetTree, link, pageId, hash);
            }
        }

        private void ValidateAnchorLink(
            IPageValidateContext ctx,
            AssetTree assetTree,
            LinkInline link,
            string url,
            string hash)
        {
            var page = assetTree.TryGetAsset(url) as PageAsset;
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