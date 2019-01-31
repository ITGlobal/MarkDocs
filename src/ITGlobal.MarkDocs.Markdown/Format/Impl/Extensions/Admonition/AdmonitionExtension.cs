using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Admonition
{
    internal sealed class AdmonitionExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.Insert(0, new AdmonitionParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<AdmonitionRenderer>();
            }
        }
    }
}