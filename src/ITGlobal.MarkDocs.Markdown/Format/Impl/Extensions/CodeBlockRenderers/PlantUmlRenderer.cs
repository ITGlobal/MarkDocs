using ITGlobal.MarkDocs.Source;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal abstract class PlantUmlRenderer : ICodeBlockRenderer
    {
        public const string Language = "plantuml";

        public bool CanRender(IPageReadContext ctx, FencedCodeBlock block)
            => block.Info == Language;

        public IRenderable CreateRenderable(IPageReadContext ctx, FencedCodeBlock block)
        {
            var markup = block.GetText();
            ctx.CreateAttachment(markup, GenerateContent(markup, block.Line), out var asset, out var url);

            return new PlantUmlRenderable(block, asset, url);
        }

        internal abstract IGeneratedAssetContent GenerateContent(string source, int? lineNumber);
    }
}