using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A parsed markdown page
    /// </summary>
    internal sealed class MarkdownPage : IParsedPage
    {
        private readonly MarkdownDocument _ast;
        private readonly MarkdownOptions _options;
        private readonly MarkdownPipeline _pipeline;
        private readonly IResourceUrlResolver _resourceUrlResolver;

        public MarkdownPage(
            MarkdownDocument ast,
            MarkdownOptions options,
            MarkdownPipeline pipeline,
            IResourceUrlResolver resourceUrlResolver,
            IReadOnlyDictionary<string, string> anchors
        )
        {
            _ast = ast;
            _options = options;
            _pipeline = pipeline;
            _resourceUrlResolver = resourceUrlResolver;
            Anchors = anchors;
        }

        /// <summary>
        ///     Page anchors (with names)
        /// </summary>
        public IReadOnlyDictionary<string, string> Anchors { get; }

        /// <summary>
        ///     Renders page into HTML
        /// </summary>
        public string Render(IRenderContext ctx)
        {
            using (MarkdownRenderingContext.SetCurrentRenderingContext(ctx, _options, _resourceUrlResolver))
            {
                // Rewrite links (and validate them)
                RewriteLinkUrls(ctx.Page, _ast);

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

        private void RewriteLinkUrls(IPage page, MarkdownDocument ast)
        {
            foreach (var link in ast.Descendants().OfType<LinkInline>())
            {
                var url = RewriteLinkUrl(page, link);
                if (url != null)
                {
                    link.Url = url;
                }
            }
        }

        [CanBeNull]
        private string RewriteLinkUrl(IPage page, LinkInline link)
        {
            var url = link.Url;
            if (url.Length > 0 && url[0] == '#')
            {
                return TryResolveSelfPageUrl(page, link, url.Substring(1));
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
                    url = NormalizeResourcePath(page, url);
                }
            }
            catch (InvalidOperationException)
            {
                MarkdownRenderingContext.RenderContext.Error(
                    $"URL \"{url}\" is not a valid relative reference",
                    link.Line
                );
                return null;
            }

            var (resolvedUrl, ok) = TryResolvePageUrl(page, link, url, hash);
            
            if (!ok)
            {
                (resolvedUrl, ok) = TryResolveAttachmentUrl(page, link, url, hash);
            }
            
            if (!ok)
            {
                MarkdownRenderingContext.RenderContext.Error(
                    $"Invalid hyperlink \"{url}\"",
                    link.Line
                );
                return null;
            }
            
            return resolvedUrl;
        }

        private string TryResolveSelfPageUrl(IPage page, LinkInline link, string hash)
        {
            if (!page.Anchors.ContainsKey(hash))
            {
                MarkdownRenderingContext.RenderContext.Error(
                    $"Anchor #{hash} doesn't exist on page \"{page.PageTreeNode.FileName}\"",
                    link.Line
                );
            }
            
            return "#" + hash;
        }

        private (string url, bool success) TryResolvePageUrl(IPage page, LinkInline link, string url, string hash)
        {
            var referencedPage = page.Documentation.GetPage(url);
            if (referencedPage == null)
            {
                return (url, false);
            }

            var resource = PseudoResource.Get(page.Documentation, url, GetResourceFileName(url), ResourceType.Page);
            url = _resourceUrlResolver.ResolveUrl(resource, page);

            if (!string.IsNullOrEmpty(hash))
            {
                if (!referencedPage.Anchors.ContainsKey(hash))
                {
                    MarkdownRenderingContext.RenderContext.Error(
                        $"Anchor #{hash} doesn't exist on page \"{page.PageTreeNode.FileName}\"",
                        link.Line
                    );
                }
                
                url += "#" + hash;
            }

            return (url, true);
        }
        
        private (string url, bool success) TryResolveAttachmentUrl(IPage page, LinkInline link, string url, string hash)
        {
            var attachment = page.Documentation.GetAttachment(url);
            if (attachment == null)
            {
                return (url, false);
            }

            if (!string.IsNullOrEmpty(hash))
            {
                MarkdownRenderingContext.RenderContext.Error(
                    $"URL \"{url}#{link}\" points to an attachment but is has a hash",
                    link.Line
                );
            }

            var resource = PseudoResource.Get(page.Documentation, url, GetResourceFileName(url), ResourceType.Attachment);
            url = _resourceUrlResolver.ResolveUrl(resource, page);
            return (url, true);
        }

        private IResource GetResourceFromUrl(IPage page, LinkInline link, string url)
        {
            try
            {
                if (!url.StartsWith("/") && !url.StartsWith("\\"))
                {
                    url = NormalizeResourcePath(page, url);
                }
            }
            catch (InvalidOperationException)
            {
                MarkdownRenderingContext.RenderContext.Error(
                    $"URL \"{url}\" is not a valid relative reference",
                    link.Line
                    );
                return null;
            }

            ResourceType type;
            if (page.Documentation.GetPage(url) != null)
            {
                type = ResourceType.Page;
            }
            else if (page.Documentation.GetAttachment(url) != null)
            {
                type = ResourceType.Attachment;
            }
            else
            {
                MarkdownRenderingContext.RenderContext.Error(
                    $"Invalid hyperlink \"{url}\"",
                    link.Line
                );
                return null;
            }

            var resource = PseudoResource.Get(page.Documentation, url, GetResourceFileName(url), type);
            return resource;
        }

        private static string NormalizeResourcePath(IPage page, string resourceUrl)
        {
            var basePath = page.Id;

            var basePathSegments = basePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var basePathLen = basePathSegments.Length;
            if (!page.PageTreeNode.IsIndexPage)
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