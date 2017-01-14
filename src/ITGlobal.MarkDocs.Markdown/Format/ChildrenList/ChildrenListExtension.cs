using Markdig;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format.ChildrenList
{
    internal sealed class ChildrenListExtension : IMarkdownExtension
    {
        private readonly IResourceUrlResolver _resourceUrlResolver;

        public ChildrenListExtension(IResourceUrlResolver resourceUrlResolver)
        {
            _resourceUrlResolver = resourceUrlResolver;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.Insert(0, new ChildrenListBlockParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.Add(new ChildrenListBlockRenderer(_resourceUrlResolver));
            }
        }
    }
}