using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Markdown.Format
{
    internal sealed class AlertBlockExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.Add(new AlertBlockParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.Add(new AlertBlockRenderer());
            }
        }
    }
}