using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Alerts
{
    internal sealed class AlertBlockExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.Add(new AlertBlockParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ObjectRenderers.Add(new AlertBlockRenderer());
            }
        }
    }
}