using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.CustomHeading
{
    internal sealed class CustomHeadingExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var i = renderer.ObjectRenderers.FindIndex(_ => _ is HeadingRenderer);
            renderer.ObjectRenderers.RemoveAt(i);
            renderer.ObjectRenderers.Insert(i, new CustomHeadingRenderer());
        }
    }
}