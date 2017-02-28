using ITGlobal.MarkDocs.Format.CodeBlockRenderers;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A PlantUML renderer based on PlantUML web service
    /// </summary>
    [PublicAPI]
    public sealed class PlantUmlWebServiceRenderer : IUmlRenderer
    {
        private readonly string _url;

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public PlantUmlWebServiceRenderer()
            : this("http://www.plantuml.com/plantuml")
        { }

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public PlantUmlWebServiceRenderer(string url)
        {
            while (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            _url = url;
        }

        /// <summary>
        ///     Render a PlantUML into an image
        /// </summary>
        public ImageData Render(string sourceCode)
        {
            var bytes = PlantUml.Render(_url, sourceCode);
            return new ImageData(bytes);
        }
    }
}