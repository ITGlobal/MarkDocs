using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

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
            var i = renderer.ObjectRenderers.FindIndex(_ => _ is HeadingRenderer);
            renderer.ObjectRenderers.RemoveAt(i);
            renderer.ObjectRenderers.Insert(i, new CustomHeadingRenderer(_dontRenderFirstHeading));
        }
    }
}