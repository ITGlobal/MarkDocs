using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.PlantUml
{
    internal sealed class PlantUmlCodeBlockRenderer : ICodeBlockRenderer
    {
        public const string Language = "plantuml";

        private readonly PlantUmlRenderer _renderer;

        public PlantUmlCodeBlockRenderer(PlantUmlRenderer renderer)
        {
            _renderer = renderer;
        }

        public IRenderable TryCreateRenderable(IPageReadContext ctx, FencedCodeBlock block)
        {
            if (block.Info != Language)
            {
                return null;
            }

            var markup = block.GetText();
            
            return _renderer.CreateRenderable(ctx, block, markup);
        }
    }
}