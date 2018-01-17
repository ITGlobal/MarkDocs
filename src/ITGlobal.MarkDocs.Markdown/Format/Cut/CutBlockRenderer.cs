using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace ITGlobal.MarkDocs.Markdown.Format.Cut
{
    internal sealed class CutBlockRenderer : HtmlObjectRenderer<CutBlock>
    {
        protected override void Write(HtmlRenderer renderer, CutBlock block) {}
    }
}