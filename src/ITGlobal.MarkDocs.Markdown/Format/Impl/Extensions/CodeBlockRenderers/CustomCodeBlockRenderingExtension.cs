using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class CustomCodeBlockRenderingExtension : IMarkdownExtension
    {
        private readonly CodeBlockRendererSelector _selector;

        public CustomCodeBlockRenderingExtension(CodeBlockRendererSelector selector)
        {
            _selector = selector;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.ReplaceOrAdd<FencedCodeBlockParser>(new CustomFencedCodeBlockParser(_selector));
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var blockRenderer = renderer.ObjectRenderers.Find<CodeBlockRenderer>();
            if (blockRenderer != null)
            {
                renderer.ObjectRenderers.Remove(blockRenderer);
            }
            
            renderer.ObjectRenderers.Add(new CustomCodeBlockRenderer());
        }
    }
}

