using System;
using ITGlobal.MarkDocs.Source;
using Markdig.Extensions.Mathematics;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.LaTeX
{
    internal sealed class CodecogsBlockRenderable : IRenderable
    {
        private readonly MathBlock _block;
        private readonly GeneratedFileAsset _asset;
        private readonly string _url;

        public CodecogsBlockRenderable(MathBlock block, GeneratedFileAsset asset, string url)
        {
            _block = block;
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
                renderer.WriteAttributes(_block);
                renderer.Write(" alt=\"\"");
                renderer.Write(" />");
            }
            catch (Exception e)
            {
                ctx.Error($"Failed to render math block. {e.Message}", _block.Line);
                renderer.WriteError("Failed to render math block");
            }
        }
    }
}