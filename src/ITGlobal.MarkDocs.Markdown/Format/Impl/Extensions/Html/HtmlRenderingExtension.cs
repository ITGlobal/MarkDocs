using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Html
{
    internal sealed class HtmlRenderingExtension : IMarkdownExtension
    {
        private readonly HtmlBlockRendererSelector _blockRendererSelector;

        public HtmlRenderingExtension(HtmlBlockRendererSelector blockRendererSelector)
        {
            _blockRendererSelector = blockRendererSelector;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.DocumentProcessed += OnDocumentProcessed;
        }

        private void OnDocumentProcessed(MarkdownDocument document)
        {
            if (!MarkdownPageReadContext.IsPresent)
            {
                return;
            }

            var context = MarkdownPageReadContext.Current;

            foreach (var descendant in document.Descendants())
            {
                if (descendant is HtmlBlock htmlBlock)
                {
                    var parsedBlock = ParsedHtmlBlock.TryCreate(htmlBlock);
                    if (parsedBlock != null)
                    {
                        var renderable = _blockRendererSelector.TryCreateRenderable(context, parsedBlock);
                        if (renderable != null)
                        {
                            htmlBlock.SetCustomRenderable(renderable);
                        }
                    }
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                var blockRenderer = htmlRenderer.ObjectRenderers.FindExact<HtmlBlockRenderer>();
                blockRenderer?.TryWriters.Insert(0, TryCustomHtmlBlockRenderer);
            }
        }

        private static bool TryCustomHtmlBlockRenderer(HtmlRenderer renderer, HtmlBlock block)
        {
            if (!MarkdownPageRenderContext.IsPresent)
            {
                return false; 
            }

            var context = MarkdownPageRenderContext.Current;

            var r = block.GetCustomRenderable();
            if (r != null)
            {
                try
                {
                    r.Render(context, renderer);
                    return true;
                }
                catch
                {
                    context.Error("Error while rendering HTML block", block.Line);
                }
            }

            return false; 
        }
    }
}