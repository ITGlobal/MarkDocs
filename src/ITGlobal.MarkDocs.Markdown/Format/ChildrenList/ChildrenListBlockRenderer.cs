using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.ChildrenList
{
    internal sealed class ChildrenListBlockRenderer : HtmlObjectRenderer<ChildrenListBlock>
    {
        protected override void Write(HtmlRenderer renderer, ChildrenListBlock block)
        {
            try
            {
                if (!MarkdownRenderingContext.IsPresent)
                {
                    return;
                }
                
                MarkdownRenderingContext.ChildrenListRenderer?.Render(renderer, block.Page);
            }
            catch (Exception e)
            {
                MarkdownRenderingContext.RenderContext?.Error($"Failed to render chilren list. {e.Message}", block.Line, e);
                renderer.WriteError("Failed to render children list");
            }
        }
    }
}
