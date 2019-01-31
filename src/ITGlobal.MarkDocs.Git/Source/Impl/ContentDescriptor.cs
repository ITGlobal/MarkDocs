using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class ContentDescriptor
    {
        private const string FILE_NAME = "content.json";

        [JsonProperty("items")]
        public Dictionary<string, ContentDescriptorItem> Items { get; set; }
            = new Dictionary<string, ContentDescriptorItem>(StringComparer.OrdinalIgnoreCase);
        
        public static ContentDescriptor LoadOrCreate(string directory, IMarkDocsLog log)
        {
            var path = Path.Combine(directory, FILE_NAME);
            if (!File.Exists(path))
            {
                return new ContentDescriptor();
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                return JsonConvert.DeserializeObject<ContentDescriptor>(json);
            }
            catch (Exception e)
            {
                log.Error(e, $"Unable to load descriptor file \"{path}\"");
                throw;
            }
        }

        public void Save(string directory, IMarkDocsLog log)
        {
            var path = Path.Combine(directory, FILE_NAME);
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(path, json, Encoding.UTF8);
            }
            catch (Exception e)
            {
                log.Error(e, $"Unable to write description file \"{path}\"");
            }
        }
    }
}