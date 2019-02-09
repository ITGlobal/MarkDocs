using System;
using System.IO;
using System.Text;
using Markdig.Syntax.Inlines;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.PlantUml
{
    internal sealed class PlantUmlImageRenderer : IImageRenderer
    {
        private readonly PlantUmlRenderer _renderer;

        public PlantUmlImageRenderer(PlantUmlRenderer renderer)
        {
            _renderer = renderer;
        }

        public IRenderable TryCreateRenderable(IPageReadContext ctx, LinkInline obj)
        {
            var url = new Uri(obj.Url, UriKind.RelativeOrAbsolute);
            if (url.IsAbsoluteUri)
            {
                return null;
            }

            var ext = Path.GetExtension(obj.Url);
            if (!string.Equals(ext, ".plantuml", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (!ctx.TryResolveFileResourcePath(obj.Url, out var path))
            {
                ctx.Error($"Unable to find file \"{obj.Url}\"", obj.Line);
                return null;
            }

            var markup = File.ReadAllText(path, Encoding.UTF8);
            return _renderer.CreateRenderable(ctx, obj, markup);
        }
    }
}