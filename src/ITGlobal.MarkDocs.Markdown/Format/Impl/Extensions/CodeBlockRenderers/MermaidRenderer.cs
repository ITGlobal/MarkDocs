using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class MermaidRenderer : ICodeBlockRenderer
    {
        public const string Language = "mermaid";
        
        public IRenderable TryCreateRenderable(IPageReadContext ctx, FencedCodeBlock block)
        {
            if (block.Info != Language)
            {
                return null;
            }

            return new MermaidRenderable(block);
        }
    }
}