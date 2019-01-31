using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.Cut
{
    internal sealed class CutBlockRenderer : HtmlObjectRenderer<CutBlock>
    {
        protected override void Write(HtmlRenderer renderer, CutBlock block) {}
    }
}