using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Search
{
    internal sealed class IndexDescriptor
    {
        private const string FILE_NAME = "index.json";

        [JsonProperty("documentations")]
        public Dictionary<string, IndexDescriptorItem> Items { get; set; }
            = new Dictionary<string, IndexDescriptorItem>(StringComparer.OrdinalIgnoreCase);

        public static IndexDescriptor LoadOrCreate(ILogger log, string directory)
        {
            var path = Path.Combine(directory, FILE_NAME);
            if (!File.Exists(path))
            {
                log.LogWarning("Description file \"{0}\" doesn't exist", path);
                return new IndexDescriptor();
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                return JsonConvert.DeserializeObject<IndexDescriptor>(json);
            }
            catch (Exception e)
            {
                log.LogError(0, e, "Unable to load descriptor file \"{0}\"", path);
                throw;
            }
        }

        public IndexDescriptorItem GetOrCreateItem(string documentationId)
        {
            IndexDescriptorItem item;
            if (!Items.TryGetValue(documentationId, out item))
            {
                item = new IndexDescriptorItem
                {
                    Path = Guid.NewGuid().ToString("N"),
                    Version = ""
                };
                Items.Add(documentationId, item);
            }

            return item;
        }

        public void Save(ILogger log, string directory)
        {
            var path = Path.Combine(directory, FILE_NAME);
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(path, json, Encoding.UTF8);
                log.LogDebug("Descriptor file \"{0}\" has been written", path);
            }
            catch (Exception e)
            {
                log.LogError(0, e, "Unable to write descriptor file \"{0}\"", path);
            }
        }
    }
}