using System.Linq;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CustomHeading
{
    internal sealed class CustomHeadingExtension : IMarkdownExtension
    {
        private readonly bool _dontRenderFirstHeading;

        public CustomHeadingExtension(bool dontRenderFirstHeading)
        {
            _dontRenderFirstHeading = dontRenderFirstHeading;
        }

        public void Setup(MarkdownPipelineBuilder pipeline) { }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<HeadingRenderer>();
                inlineRenderer?.TryWriters.Insert(0, TryCustomHeadingRenderer);
            }
        }

        private bool TryCustomHeadingRenderer(HtmlRenderer renderer, HeadingBlock block)
        {
            if (_dontRenderFirstHeading)
            {
                var document = block.GetDocument();
                if (document?.OfType<HeadingBlock>()
                        .Where((b, i) => b == block && i == 0)
                        .FirstOrDefault() != null)
                {
                    return true;
                }
            }
            
            if (block.IsNoRender())
            {
                return true;
            }

            return false;
        }
    }
}