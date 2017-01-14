using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Git
{
    internal sealed class ContentDescriptor
    {
        private const string FILE_NAME = "content.json";

        [JsonProperty("versions")]
        public Dictionary<string, ContentDescriptorItem> Items { get; set; }
        = new Dictionary<string, ContentDescriptorItem>(StringComparer.OrdinalIgnoreCase);

        public static ContentDescriptor LoadOrCreate(ILogger log, string directory)
        {
            var path = Path.Combine(directory, FILE_NAME);
            if (!File.Exists(path))
            {
                log.LogWarning("Description file \"{0}\" doesn't exist", path);
                return new ContentDescriptor();
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                return JsonConvert.DeserializeObject<ContentDescriptor>(json);
            }
            catch (Exception e)
            {
                log.LogError(0, e, "Unable to load descriptor file \"{0}\"", path);
                throw;
            }
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
                log.LogError(0, e, "Unable to write description file \"{0}\"", path);
            }
        }
    }
}