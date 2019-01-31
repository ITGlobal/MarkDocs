using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Icons
{
    internal sealed class IconExtension : IMarkdownExtension
    {
        // TODO add support for fortawesome
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.InlineParsers.Insert(0, new IconParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer && !htmlRenderer.ObjectRenderers.Contains<IconInlineRenderer>())
            {
                htmlRenderer.ObjectRenderers.Insert(0, new IconInlineRenderer());
            }
        }
    }
}