using AngleSharp.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ITGlobal.MarkDocs.Search.Impl
{
    internal sealed class IndexDescriptor
    {
        private const string FILE_NAME = "index.json";

        [JsonProperty("documentations")]
        public Dictionary<string, IndexDescriptorItem> Items { get; set; }
            = new Dictionary<string, IndexDescriptorItem>(StringComparer.OrdinalIgnoreCase);

        public static IndexDescriptor LoadOrCreate(string directory, IMarkDocsLog log)
        {
            var path = Path.Combine(directory, FILE_NAME);
            if (!File.Exists(path))
            {
                return new IndexDescriptor();
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                return JsonConvert.DeserializeObject<IndexDescriptor>(json);
            }
            catch (Exception e)
            {
                log.Error(e, "Unable to load search index descriptor file \"{path}\"");
                throw;
            }
        }

        public IndexDescriptorItem GetOrCreateItem(string documentationId)
        {
            if (!Items.TryGetValue(documentationId, out var item))
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

        public void Save(string directory, IMarkDocsLog log)
        {
            var path = Path.Combine(directory, FILE_NAME);
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(path, json, Encoding.UTF8);
                log.Debug($"Search index descriptor file \"{path}\" has been written");
            }
            catch (Exception e)
            {
                log.Error(e, $"Unable to write search index descriptor file \"{path}\"");
            }
        }

        public IndexDescriptor Clone()
        {
            return new IndexDescriptor
            {
                Items = Items.ToDictionary(
                    _ => _.Key,
                    _ => new IndexDescriptorItem
                    {
                        Path = _.Value.Path,
                        Version = _.Value.Version,
                    }
                )
            };
        }
    }
}
