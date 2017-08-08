using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.Extensions.Logging;
using static ITGlobal.MarkDocs.Format.MarkdownRenderingContext;

namespace ITGlobal.MarkDocs.Format.ChildrenList
{
    internal sealed class ChildrenListBlockRenderer : HtmlObjectRenderer<ChildrenListBlock>
    {
        protected override void Write(HtmlRenderer renderer, ChildrenListBlock block)
        {
            try
            {
                ChildrenListRenderer?.Render(renderer, block.Page);
            }
            catch (Exception exception)
            {
                Logger.LogError(0, exception, "Error while rendering {0}", nameof(ChildrenListBlock));
                renderer.WriteError("Failed to render children list");
            }
        }
    }
}
