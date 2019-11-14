using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class CustomCodeBlockRenderingExtension : IMarkdownExtension
    {
        private readonly CodeBlockRendererSelector _selector;

        public CustomCodeBlockRenderingExtension(CodeBlockRendererSelector selector)
        {
            _selector = selector;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.ReplaceOrAdd<FencedCodeBlockParser>(new CustomFencedCodeBlockParser(_selector));
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                var inlineRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();
                inlineRenderer?.TryWriters.Insert(0, TryCustomCodeBlockRenderer);
            }
        }

        private static bool TryCustomCodeBlockRenderer(HtmlRenderer renderer, CodeBlock block)
        {
            if (!MarkdownPageRenderContext.IsPresent)
            {
                return false;
            }

            var context = MarkdownPageRenderContext.Current;
            if (block is FencedCodeBlock codeBlock)
            {
                var r = codeBlock.GetCustomRenderable();
                if (r != null)
                {
                    try
                    {
                        r.Render(context, renderer);
                        return true;
                    }
                    catch
                    {
                        context.Error("Error while rendering code block", block.Line);
                    }
                }
            }

            return false;
        }
    }
}

