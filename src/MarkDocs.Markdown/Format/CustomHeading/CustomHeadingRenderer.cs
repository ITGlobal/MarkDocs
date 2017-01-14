using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.CustomHeading
{
    internal sealed class CustomHeadingRenderer : HeadingRenderer
    {
        private const string NO_RENDER_PROPERTY_KEY = "no-render";

        public static void DontRender(HeadingBlock block) => block.SetData(NO_RENDER_PROPERTY_KEY, "");

        protected override void Write(HtmlRenderer renderer, HeadingBlock obj)
        {
            if (obj.GetData(NO_RENDER_PROPERTY_KEY) != null)
            {
                return;
            }

            base.Write(renderer, obj);
        }
    }
}