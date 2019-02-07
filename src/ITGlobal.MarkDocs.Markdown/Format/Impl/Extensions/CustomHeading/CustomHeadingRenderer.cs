using System.Linq;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CustomHeading
{
    internal sealed class CustomHeadingRenderer : HeadingRenderer
    {
        private readonly bool _dontRenderFirstHeading;

        public CustomHeadingRenderer(bool dontRenderFirstHeading)
        {
            _dontRenderFirstHeading = dontRenderFirstHeading;
        }
        
        protected override void Write(HtmlRenderer renderer, HeadingBlock block)
        {
            if (_dontRenderFirstHeading)
            {
                var document = block.GetDocument();
                if (document?.OfType<HeadingBlock>()
                        .Where((b, i) => b == block && i == 0)
                        .FirstOrDefault() != null)
                {
                    return;
                }
            }
            
            if (block.IsNoRender())
            {
                return;
            }

            base.Write(renderer, block);
        }
    }
}