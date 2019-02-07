using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Source;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class NoneChildrenListRenderer : IChildrenListRenderer
    {
        public void Render(HtmlRenderer renderer, AssetTree assetTree, PageAsset page) { }
    }
}
