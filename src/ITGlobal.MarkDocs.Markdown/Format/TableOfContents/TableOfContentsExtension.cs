using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.TableOfContents
{
    internal sealed class TableOfContentsExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.Insert(0, new TableOfContentsBlockParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<TableOfContentsBlockRenderer>();
            }
        }
    }
}