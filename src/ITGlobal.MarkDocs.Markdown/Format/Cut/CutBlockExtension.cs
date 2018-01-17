using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Markdown.Format.Cut
{
    internal sealed class CutBlockExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.Insert(0, new CutBlockParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<CutBlockRenderer>();
            }
        }
    }
}