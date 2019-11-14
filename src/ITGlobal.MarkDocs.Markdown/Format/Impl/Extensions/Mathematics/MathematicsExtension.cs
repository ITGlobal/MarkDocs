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
            if (renderer is HtmlRenderer htmlRenderer)
            {
                var htmlMathInlineRenderer = htmlRenderer.ObjectRenderers.FindExact<HtmlMathInlineRenderer>();
                htmlMathInlineRenderer?.TryWriters.Insert(0, TryCustomMathInlineRenderer);

                var htmlMathBlockRenderer = htmlRenderer.ObjectRenderers.FindExact<HtmlMathBlockRenderer>();
                htmlMathBlockRenderer?.TryWriters.Insert(0, TryCustomMathInlineRenderer);
            }
        }

        private static bool TryCustomMathInlineRenderer(HtmlRenderer renderer, MathInline inline)
        {
            if (MarkdownPageRenderContext.IsPresent)
            {
                var r = inline.GetCustomRenderable();
                if (r != null)
                {
                    var context = MarkdownPageRenderContext.Current;
                    try
                    {
                        r.Render(context, renderer);
                        return true;
                    }
                    catch
                    {
                        context.Error("Error while rendering math inline", inline.Line);
                    }
                }
            }

            return false;
        }

        private static bool TryCustomMathInlineRenderer(HtmlRenderer renderer, MathBlock block)
        {
            if (MarkdownPageRenderContext.IsPresent)
            {
                var r = block.GetCustomRenderable();
                if (r != null)
                {
                    var context = MarkdownPageRenderContext.Current;
                    try
                    {
                        r.Render(context, renderer);
                        return true;
                    }
                    catch
                    {
                        context.Error("Error while rendering math block", block.Line);
                    }
                }
            }

            return false;
        }
    }
}
