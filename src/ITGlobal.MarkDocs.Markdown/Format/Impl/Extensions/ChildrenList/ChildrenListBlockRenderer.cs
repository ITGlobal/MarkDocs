using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.ChildrenList
{
    internal sealed class ChildrenListBlockRenderer : HtmlObjectRenderer<ChildrenListBlock>
    {
        private readonly IChildrenListRenderer _renderer;

        public ChildrenListBlockRenderer(IChildrenListRenderer renderer)
        {
            _renderer = renderer;
        }

        protected override void Write(HtmlRenderer renderer, ChildrenListBlock block)
        {
            if (!MarkdownPageRenderContext.IsPresent)
            {
                return;
            }


            var ctx = MarkdownPageRenderContext.Current;
            try
            {
                _renderer.Render(renderer, ctx.AssetTree, ctx.Page);
            }
            catch (Exception e)
            {
                ctx.Error($"Failed to render children list. {e.Message}", block.Line);
                renderer.WriteError("Failed to render children list");
            }
        }
    }
}
