using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.ChildrenList
{
    internal sealed class ChildrenListExtension : IMarkdownExtension
    {
        private readonly IChildrenListRenderer _renderer;

        public ChildrenListExtension(IChildrenListRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.Insert(0, new ChildrenListBlockParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ObjectRenderers.Add(new ChildrenListBlockRenderer(_renderer));
            }
        }
    }
}