using System;
using ITGlobal.MarkDocs.Source;
using Markdig.Extensions.Mathematics;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.LaTeX
{
    internal sealed class CodecogsInlineRenderable : IRenderable
    {
        private readonly MathInline _inline;
        private readonly GeneratedFileAsset _asset;
        private readonly string _url;

        public CodecogsInlineRenderable(MathInline inline, GeneratedFileAsset asset, string url)
        {
            _inline = inline;
            _asset = asset;
            _url = url;
        }

        public void Render(IPageRenderContext ctx, HtmlRenderer renderer)
        {
            try
            {
                ctx.Store(_asset);

                renderer.EnsureLine();
                renderer.Write("<img src=\"");
                renderer.WriteEscapeUrl(_url);
                renderer.Write("\"");
                renderer.WriteAttributes(_inline);
                renderer.Write(" alt=\"\"");
                renderer.Write(" />");
            }
            catch (Exception e)
            {
                ctx.Error($"Failed to render math inline. {e.Message}", _inline.Line);
                renderer.WriteError("Failed to render math inline");
            }
        }
    }
}