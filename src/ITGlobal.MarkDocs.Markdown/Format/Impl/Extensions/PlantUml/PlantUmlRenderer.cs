using ITGlobal.MarkDocs.Source;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.PlantUml
{
    internal abstract class PlantUmlRenderer 
    {
        public const string Language = "plantuml";

        public IRenderable CreateRenderable(IPageReadContext ctx, MarkdownObject obj, string markup)
        {
            ctx.CreateAttachment(markup, GenerateContent(markup, obj.Line), out var asset, out var url);

            return new PlantUmlRenderable(obj, asset, url);
        }

        internal abstract IGeneratedAssetContent GenerateContent(string source, int? lineNumber);
    }
}