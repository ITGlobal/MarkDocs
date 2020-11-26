using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache.Model
{
    /// <summary>
    ///     Documentation model
    /// </summary>
    [PublicAPI]
    public sealed class DocumentationModel
    {

        /// <summary>
        ///     Documentation ID
        /// </summary>
        [JsonProperty("id")]
        [NotNull]
        public string Id { get; set; }

        /// <summary>
        ///     Documentation root page
        /// </summary>
        [JsonProperty("root_page")]
        [NotNull]
        public PageModel RootPage { get; set; }

        /// <summary>
        ///     List of file resources
        /// </summary>
        [JsonProperty("files")]
        [NotNull]
        public FileModel[] Files { get; set; }

        /// <summary>
        ///     Source info
        /// </summary>
        [JsonProperty("info")]
        [NotNull]
        public SourceInfoModel Info { get; set; }

        /// <summary>
        ///     Compilation report
        /// </summary>
        [JsonProperty("compilation_report", NullValueHandling = NullValueHandling.Ignore)]
        [NotNull]
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