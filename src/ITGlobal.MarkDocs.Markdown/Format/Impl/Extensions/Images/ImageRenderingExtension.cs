using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Images
{
    internal sealed class ImageRenderingExtension : IMarkdownExtension
    {
        private readonly ImageRendererSelector _selector;

        public ImageRenderingExtension(ImageRendererSelector selector)
        {
            _selector = selector;
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
                if (descendant is LeafBlock leaf && leaf.Inline !=null)
                {
                    foreach (var inline in leaf.Inline)
                    {
                        if (inline is LinkInline link && link.IsImage)
                        {
                            var renderable = _selector.TryCreateRenderable(context, link);
                            if (renderable != null)
                            {
                                inline.SetCustomRenderable(renderable);
                            }
                        }
                    }
                }
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var r = renderer.ObjectRenderers.Find<LinkInlineRenderer>();
            if (r != null)
            {
                renderer.ObjectRenderers.Remove(r);
            }

            renderer.ObjectRenderers.Add(new CustomLinkInlineRenderer());
        }
    }
}
