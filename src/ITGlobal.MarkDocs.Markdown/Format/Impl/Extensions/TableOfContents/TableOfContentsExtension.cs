using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.TableOfContents
{
    internal sealed class TableOfContentsExtension : IMarkdownExtension
    {
        private readonly ITocRenderer _renderer;

        public TableOfContentsExtension(ITocRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.Insert(0, new TableOfContentsBlockParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new TableOfContentsBlockRenderer(_renderer));
            }
        }
    }
}