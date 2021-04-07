using System;
using System.Collections.Generic;
using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CustomBlockRendering
{
    internal sealed class CustomBlockRenderingExtension : IMarkdownExtension
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly List<Action<IServiceProvider, HtmlRenderer>> _htmlRendererOverrides;

        public CustomBlockRenderingExtension(
            IServiceProvider serviceProvider,
            List<Action<IServiceProvider, HtmlRenderer>> htmlRendererOverrides)
        {
            _serviceProvider = serviceProvider;
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
                func(_serviceProvider, htmlRenderer);
            }
        }

    }
}