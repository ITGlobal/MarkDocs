using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.CodeBlockRenderers
{
    internal sealed class CustomCodeBlockRenderingExtension : IMarkdownExtension
    {
        private readonly MarkdownOptions _options;

        public CustomCodeBlockRenderingExtension(MarkdownOptions options)
        {
            _options = options;
        }

        public void Setup(MarkdownPipelineBuilder pipeline) { }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var blockRenderer = renderer.ObjectRenderers.Find<CodeBlockRenderer>();
            if (blockRenderer != null)
            {
                renderer.ObjectRenderers.Remove(blockRenderer);
            }

            renderer.ObjectRenderers.Add(new CustomCodeBlockRenderer(_options));
        }
    }
}

