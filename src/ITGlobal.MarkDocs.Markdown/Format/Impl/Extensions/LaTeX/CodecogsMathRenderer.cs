using Markdig.Extensions.Mathematics;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.LaTeX
{
    /// <summary>
    ///     A MathML/Tex/LaTex renderer that uses http://latex.codecogs.com
    /// </summary>
    internal sealed class CodecogsMathRenderer : IMathRenderer
    {
        public const string DefaultUrl = "http://latex.codecogs.com/png.download";

        private readonly string _url;

        public CodecogsMathRenderer(string url)
        {
            _url = url;
        }

        public IRenderable CreateRenderable(IPageReadContext ctx, MathBlock block)
        {
            var markup = block.GetText();

            ctx.CreateAttachment(
                markup,
                new CodecogsGeneratedAsset(_url, markup, block.Line),
                out var asset,
                out var url
            );
            return new CodecogsBlockRenderable(block, asset, url);
        }

        public IRenderable CreateRenderable(IPageReadContext ctx, MathInline inline)
        {
            var markup = inline.GetText();

            ctx.CreateAttachment(
                markup,
                new CodecogsGeneratedAsset(_url, markup, inline.Line),
                out var asset,
                out var url
            );
            return new CodecogsInlineRenderable(inline, asset, url);
        }
    }
}