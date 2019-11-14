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
            var u = new Uri(obj.Url, UriKind.RelativeOrAbsolute);
            if (u.IsAbsoluteUri)
            {
                return null;
            }

            var url = obj.Url;
            url = MarkdownPageContent.NormalizeResourcePath(ctx, url);
            
            var ext = Path.GetExtension(obj.Url);
            if(!PlantUmlRenderer.SupportedFileExtensions.Contains(ext))
            {
                return null;
            }

            if (!ctx.TryResolveFileResourcePath(url, out var path))
            {
                ctx.Error($"Unable to find file \"{obj.Url}\"", obj.Line);
                return null;
            }

            var filename = Path.Combine(
                Path.GetDirectoryName(url),
                Path.ChangeExtension(Path.GetFileName(url), ".png")
            );

            var markup = File.ReadAllText(path, Encoding.UTF8);
            return _renderer.CreateRenderable(ctx, obj, markup, filename);
        }
    }
}