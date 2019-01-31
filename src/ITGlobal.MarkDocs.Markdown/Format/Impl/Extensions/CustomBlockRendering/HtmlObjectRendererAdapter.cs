using System;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CustomBlockRendering
{
    internal sealed class HtmlObjectRendererAdapter<T> : HtmlObjectRenderer<T>
        where T: MarkdownObject
    {
        private readonly Func<HtmlRenderer, T, bool> _renderer;

        public HtmlObjectRendererAdapter(Func<HtmlRenderer, T, bool> renderer)
        {
            _renderer = renderer;
        }

        protected override void Write(HtmlRenderer renderer, T block) => _renderer(renderer, block);
    }
}