using Markdig;
using Markdig.Extensions.Mathematics;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Mathematics
{
    internal sealed class MathematicsExtension : IMarkdownExtension
    {
        private readonly IMathRenderer _renderer;

        public MathematicsExtension(IMathRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.InlineParsers.ReplaceOrAdd<MathInlineParser>(new CustomMathInlineParser(_renderer));
            pipeline.BlockParsers.ReplaceOrAdd<MathBlockParser>(new CustomMathBlockParser(_renderer));
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer == null)
            {
                return;
            }

            htmlRenderer.ObjectRenderers.ReplaceOrAdd<HtmlMathInlineRenderer>(new CustomMathInlineRenderer());
            htmlRenderer.ObjectRenderers.ReplaceOrAdd<HtmlMathBlockRenderer>(new CustomMathBlockRenderer());
        }
    }
}
