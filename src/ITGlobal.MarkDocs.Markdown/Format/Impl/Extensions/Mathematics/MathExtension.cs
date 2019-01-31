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
            if (!pipeline.InlineParsers.Contains<MathInlineParser>())
            {
                pipeline.InlineParsers.Insert(0, new MathInlineParser());
            }

            if (pipeline.BlockParsers.Contains<MathBlockParser>())
            {
                return;
            }

            pipeline.BlockParsers.Insert(0, new MathBlockParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer == null)
            {
                return;
            }

            htmlRenderer.ObjectRenderers.Insert(0, new MathematicsInlineRenderer(_renderer));
            htmlRenderer.ObjectRenderers.Insert(0, new MathematicsBlockRenderer(_renderer));
        }
    }
}
