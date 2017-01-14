using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Icons
{
    internal sealed class IconExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.InlineParsers.Insert(0, new IconParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<IconInlineRenderer>())
            {
                htmlRenderer.ObjectRenderers.Insert(0, new IconInlineRenderer());
            }
        }
    }
}