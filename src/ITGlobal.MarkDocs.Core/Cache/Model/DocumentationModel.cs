using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    public sealed class DocumentationModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("root_page")]
        public PageModel RootPage { get; set; }

        [JsonProperty("files")]
        public FileModel[] Files { get; set; }

        [JsonProperty("info")]
        public SourceInfoModel Info { get; set; }

        [JsonProperty("compilation_report", NullValueHandling = NullValueHandling.Ignore)]
        public CompilationReportModel CompilationReport { get; set; }

        internal const string FILE_NAME = "model.json";

        internal static DocumentationModel Load(string path, IMarkDocsLog log)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                var model = JsonConvert.DeserializeObject<DocumentationModel>(json);
                return model;
            }
            catch (Exception e)
            {
                log.Error(e, $"Unable to load documentation model from '{path}'");
                return null;
            }
        }

        internal void Save(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}
