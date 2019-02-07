using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class MermaidRenderer : ICodeBlockRenderer
    {
        public const string Language = "mermaid";
        
        public bool CanRender(IPageReadContext ctx, FencedCodeBlock block) 
            => block.Info == Language;

        public IRenderable CreateRenderable(IPageReadContext ctx, FencedCodeBlock block)
        {
            return new MermaidRenderable(block);
        }
    }
}