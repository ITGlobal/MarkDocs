using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Markdown.Format.Admonition
{
    internal sealed class AdmonitionExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.Insert(0, new AdmonitionParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<AdmonitionRenderer>();
            }
        }
    }
}