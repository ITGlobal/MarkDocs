using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.TableOfContents
{
    internal sealed class TableOfContentsBlockRenderer : HtmlObjectRenderer<TableOfContentsBlock>
    {
        private readonly ITocRenderer _renderer;

        public TableOfContentsBlockRenderer(ITocRenderer renderer)
        {
            _renderer = renderer;
        }

        protected override void Write(HtmlRenderer renderer, TableOfContentsBlock block)
        {
            try
            {
                if (!MarkdownRenderingContext.IsPresent)
                {
                    return;
                }

                var page = MarkdownRenderingContext.RenderContext.Page;
                if (_renderer != null && page.Content.Anchors != null && page.Content.Anchors.Count > 0)
                {
                    _renderer.Render(renderer, page.Content.Anchors);
                }
            }
            catch (Exception e)
            {
                MarkdownRenderingContext.RenderContext?.Error($"Failed to render table of contents. {e.Message}", block.Line);
                renderer.WriteError("Failed to render table of contents");
            }
        }
    }
}