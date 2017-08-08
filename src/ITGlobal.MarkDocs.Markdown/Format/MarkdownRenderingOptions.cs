using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Options for rendering markdown into HTML
    /// </summary>
    [PublicAPI]
    public sealed class MarkdownRenderingOptions : IMarkdownExtension
    {
        private readonly List<Action<HtmlRenderer>> _htmlRendererOverrides = new List<Action<HtmlRenderer>>();
        
        internal MarkdownRenderingOptions()
        {
        }

        /// <summary>
        ///     Sets custom rendering function for <typeparamref name="T"/>
        /// </summary>
        [PublicAPI]
        public MarkdownRenderingOptions OverrideRendering<T>(Action<HtmlRenderer, T> renderer)
            where T : MarkdownObject
        {
            _htmlRendererOverrides.Add(htmlRenderer =>
            {
                var existing = htmlRenderer.ObjectRenderers.FirstOrDefault(_ => _ is HtmlObjectRenderer<T>);
                if (existing != null)
                {
                    var index = htmlRenderer.ObjectRenderers.IndexOf(existing);
                    htmlRenderer.ObjectRenderers.Insert(index, new HtmlObjectRendererImpl<T>(renderer));
                    htmlRenderer.ObjectRenderers.Remove(existing);
                }
                else
                {
                    htmlRenderer.ObjectRenderers.Add(new HtmlObjectRendererImpl<T>(renderer));
                }
            });

            return this;
        }

        private sealed class HtmlObjectRendererImpl<T> : HtmlObjectRenderer<T>
            where T : MarkdownObject
        {
            private readonly Action<HtmlRenderer, T> _renderer;

            public HtmlObjectRendererImpl(Action<HtmlRenderer, T> renderer)
            {
                _renderer = renderer;
            }

            protected override void Write(HtmlRenderer renderer, T block) => _renderer(renderer, block);
        }

        void IMarkdownExtension.Setup(MarkdownPipelineBuilder pipeline)
        { }

        void IMarkdownExtension.Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer == null)
            {
                return;
            }

            foreach (var func in _htmlRendererOverrides)
            {
                func(htmlRenderer);
            }
        }
    }
}