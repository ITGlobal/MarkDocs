using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using Markdig.Renderers;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     HTML renderer for Child pages list
    /// </summary>
    [PublicAPI]
    public interface IChildrenListRenderer
    {
        /// <summary>
        ///     Renders a list of child pages into HTML
        /// </summary>
        void Render([NotNull] HtmlRenderer renderer, [NotNull] AssetTree assetTree, [NotNull] PageAsset page);
    }
}