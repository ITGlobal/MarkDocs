using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.Impl
{
    internal static class MarkdownProperties
    {
        private const string NO_RENDER_PROPERTY_KEY = "no-render";

        public static void SetNoRender(this HeadingBlock block)
            => block.SetData(NO_RENDER_PROPERTY_KEY, "");
        public static bool IsNoRender(this HeadingBlock block)
            => block.GetData(NO_RENDER_PROPERTY_KEY) != null;

        private const string CUSTOM_RENDERABLE_PROPERTY_KEY = "custom-renderable";

        public static void SetCustomRenderable(this MarkdownObject block, IRenderable renderable)
            => block.SetData(CUSTOM_RENDERABLE_PROPERTY_KEY, renderable);
        public static IRenderable GetCustomRenderable(this MarkdownObject block)
            => block.GetData(CUSTOM_RENDERABLE_PROPERTY_KEY) as IRenderable;

        private const string REWRITTEN_LINK_PROPERTY_KEY = "custom-renderable";

        public static void SetIsRewrittenLink(this MarkdownObject block)
            => block.SetData(REWRITTEN_LINK_PROPERTY_KEY, "");
        public static bool IsRewrittenLink(this MarkdownObject block)
            => block.GetData(REWRITTEN_LINK_PROPERTY_KEY) != null;
    }
}