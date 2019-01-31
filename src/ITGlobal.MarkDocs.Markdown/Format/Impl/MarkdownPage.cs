using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.IO;
using System.Linq;

namespace ITGlobal.MarkDocs.Format.Impl
{
    /// <summary>
    ///     A parsed markdown page
    /// </summary>
    internal sealed class MarkdownPage : IParsedPage
    {
        private readonly MarkdownDocument _ast;
        private readonly MarkdownDocument _previewAst;
        private readonly MarkdownPipeline _pipeline;
        private readonly IResourceUrlResolver _resourceUrlResolver;

        public MarkdownPage(
            MarkdownDocument ast,
            MarkdownDocument previewAst,
            MarkdownPipeline pipeline,
            IResourceUrlResolver resourceUrlResolver,
            PageAnchor[] anchors
        )
        {
            _ast = ast;
            _previewAst = previewAst;
            _pipeline = pipeline;
            _resourceUrlResolver = resourceUrlResolver;
            Anchors = new PageAnchors(anchors);
        }

        public PageAnchors Anchors { get; }

        /// <summary>
        ///     true if page contains a "cut" separator
        /// </summary>
        public bool HasPreview => _previewAst != null;

        /// <summary>
        ///     Renders page into HTML
        /// </summary>
        public string Render(IPageRenderContext ctx)
        {
            return Render(ctx, _ast);
        }

        /// <summary>
        ///     Renders page preview into HTML
        /// </summary>
        public string RenderPreview(IPageRenderContext ctx)
        {
            if (_previewAst == null)
            {
                throw new Exception("This page doesn't have a preview");
            }

            return Render(ctx, _previewAst);
        }

        private string Render(IPageRenderContext ctx, MarkdownDocument ast)
        {
            using (MarkdownRenderingContext.SetCurrentRenderingContext(ctx, _resourceUrlResolver))
            {
                // Rewrite links (and validate them)
                RewriteLinkUrls(ctx, ast);

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

        private void RewriteLinkUrls(IPageRenderContext ctx, MarkdownDocument ast)
        {
            foreach (var link in ast.Descendants().OfType<LinkInline>())
            {
                var url = RewriteLinkUrl(ctx, link);
                if (url != null)
                {
                    link.Url = url;
                }
            }
        }

        private const string REWRITTEN_LINK_PROP = "RewrittenLinkProp";

        [CanBeNull]
        private string RewriteLinkUrl(IPageRenderContext ctx, LinkInline link)
        {
            if (link.ContainsData(REWRITTEN_LINK_PROP))
            {
                return null;
            }

            link.SetData(REWRITTEN_LINK_PROP, "");

            var url = link.Url;
            if (url.Length > 0 && url[0] == '#')
            {
                return TryResolveSelfPageUrl(ctx, link, url.Substring(1));
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
                    url = NormalizeResourcePath(ctx.Page, url);
                }
            }
            catch (InvalidOperationException)
            {
                ctx.Error(
                    $"URL \"{url}\" is not a valid relative reference",
                    link.Line
                );
                return null;
            }

            var (resolvedUrl, ok) = TryResolveAssetUrl(ctx, link, url, hash);
            
            if (!ok)
            {
                ctx.Error($"Invalid hyperlink \"{url}\"", link.Line);
                return null;
            }

            return resolvedUrl;
        }

        private string TryResolveSelfPageUrl(IPageRenderContext ctx, LinkInline link, string hash)
        {
            if (ctx.Page.Content.Anchors == null || ctx.Page.Content.Anchors[hash] == null)
            {
                ctx.Error(
                    $"Anchor #{hash} doesn't exist on page \"{ctx.Page.RelativePath}\"",
                    link.Line
                );
            }

            return "#" + hash;
        }

        private (string url, bool success) TryResolveAssetUrl(IPageRenderContext ctx, LinkInline link, string url, string hash)
        {
            var referencedAsset = ctx.AssetTree.TryGetAsset(url);
            if (referencedAsset == null)
            {
                return (url, false);
            }

            switch (referencedAsset)
            {
                case PageAsset referencedPage:
                    return (ResolvePageUrl(ctx, referencedPage, link, url, hash), true);

                case AttachmentAsset referencedAttachment:
                    return (ResolveAttachmentUrl(ctx, referencedAttachment, link, url, hash), true);

                default:
                    return (url, false);
            }
        }

        private string ResolvePageUrl(
            IPageRenderContext ctx,
            PageAsset referencedPage,
            LinkInline link,
            string url,
            string hash
            )
        {
            var resource = PseudoResource.Get(url, GetResourceFileName(url), ResourceType.Page);
            url = _resourceUrlResolver.ResolveUrl(ctx, resource);

            if (!string.IsNullOrEmpty(hash))
            {
                if (referencedPage.Content.Anchors == null ||
                    referencedPage.Content.Anchors[hash] == null)
                {
                    ctx.Error(
                        $"Anchor #{hash} doesn't exist on page \"{referencedPage.RelativePath}\"",
                        link.Line
                    );
                }
                else if (referencedPage.Id == ctx.Page.Id)
                {
                    var text = link.GetText();
                    ctx.Warning(
                        $"Link [{text}]({url}#{hash}) can be replaced with {text}](#{hash})",
                        link.Line
                    );
                }

                url += "#" + hash;
            }

            return url;
        }

        private string ResolveAttachmentUrl(
            IPageRenderContext ctx,
            AttachmentAsset referencedAttachment,
            LinkInline link,
            string url,
            string hash)
        {
            if (!string.IsNullOrEmpty(hash))
            {
                ctx.Error(
                    $"URL \"{url}#{link}\" points to an attachment but is has a hash",
                    link.Line
                );
            }

            url = _resourceUrlResolver.ResolveUrl(ctx, referencedAttachment);
            return url;
        }
        
        private static string NormalizeResourcePath(PageAsset page, string resourceUrl)
        {
            var basePath = page.Id;

            var basePathSegments = basePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var basePathLen = basePathSegments.Length;
            //if (!page.PageTreeNode.IsIndexPage)
            //{
            //    basePathLen--;
            //}
            if (!(page is BranchPageAsset))
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

        private static string GetResourceFileName(string url)
        {
            var filename = Path.GetFileName(url);

            var ext = Path.GetExtension(filename);
            if (ext == ".md" || ext == ".markdown")
            {
                filename = Path.ChangeExtension(filename, ".html");
            }

            return filename;
        }

    }
}