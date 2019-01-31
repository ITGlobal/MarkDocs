using System;
using System.Collections.Generic;
using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CustomBlockRendering
{
    internal sealed class CustomBlockRenderingExtension : IMarkdownExtension
    {
        private readonly List<Action<HtmlRenderer>> _htmlRendererOverrides;

        public CustomBlockRenderingExtension(List<Action<HtmlRenderer>> htmlRendererOverrides)
        {
            _htmlRendererOverrides = htmlRendererOverrides;
        }

        void IMarkdownExtension.Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        void IMarkdownExtension.Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (!(renderer is HtmlRenderer htmlRenderer))
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
