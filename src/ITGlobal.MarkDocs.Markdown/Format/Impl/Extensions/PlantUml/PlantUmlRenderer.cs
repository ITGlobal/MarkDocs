using System;
using System.Collections.Immutable;
using ITGlobal.MarkDocs.Source;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.PlantUml
{
    internal abstract class PlantUmlRenderer 
    {
        public const string Language = "plantuml";

        public static readonly ImmutableHashSet<string> SupportedFileExtensions =
            ImmutableHashSet.CreateRange(
                StringComparer.OrdinalIgnoreCase,
                new[] {".wsd", ".pu", ".puml", ".plantuml", ".iuml"}
            );

        public IRenderable CreateRenderable(IPageReadContext ctx, MarkdownObject obj, string markup, string filename = null)
        {
            GeneratedFileAsset asset;
            string url;

            if (!string.IsNullOrEmpty(filename))
            {
                ctx.CreateAttachment(markup, filename, GenerateContent(markup, obj.Line), out asset, out url);
            }
            else
            {
                ctx.CreateAttachment(markup, GenerateContent(markup, obj.Line), out asset, out url);
            }

            return new PlantUmlRenderable(obj, asset, url);
        }

        internal abstract IGeneratedAssetContent GenerateContent(string source, int? lineNumber);
    }
}