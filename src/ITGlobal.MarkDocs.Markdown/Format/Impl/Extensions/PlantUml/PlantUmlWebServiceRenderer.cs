using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.PlantUml
{
    internal sealed class PlantUmlWebServiceRenderer : PlantUmlRenderer
    {
        public const string DefaultUrl = "http://www.plantuml.com/plantuml";

        private readonly string _url;

        public PlantUmlWebServiceRenderer(string url)
        {
            _url = url;
        }

        internal override IGeneratedAssetContent GenerateContent(string source, int? lineNumber)
        {
            return new PlantUmlGeneratedContent(_url, source, lineNumber);
        }
    }
}