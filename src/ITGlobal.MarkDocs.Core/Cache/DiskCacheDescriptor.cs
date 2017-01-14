using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Disk cache descriptor
    /// </summary>
    internal sealed class DiskCacheDescriptor
    {
        #region properties

        /// <summary>
        ///     Last cache update time
        /// </summary>
        [JsonProperty("lastUpdated")]
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        ///     Cache descriptor items
        /// </summary>
        [JsonProperty("items")]
        public Dictionary<string, DiskCacheDocumentationDescriptor> Items { get; set; }
            = new Dictionary<string, DiskCacheDocumentationDescriptor>(StringComparer.OrdinalIgnoreCase);
        
        #endregion

        #region methods

        /// <summary>
        ///     Loads a cache descriptor from a file or creates a new one
        /// </summary>
        public static DiskCacheDescriptor LoadOrCreate(string path, ILogger log)
        {
            DiskCacheDescriptor descriptor = null;

            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path, Encoding.UTF8);
                    descriptor = JsonConvert.DeserializeObject<DiskCacheDescriptor>(json);
                    log.LogDebug("File '{0}' has been loaded", path);
                }
                catch (Exception e)
                {
                    log.LogError(0, e, "Failed to load file '{0}'", path);
                    descriptor = null;
                }
            }

            if (descriptor == null)
            {
                descriptor = new DiskCacheDescriptor();
                log.LogDebug("Created empty cache descriptor");
            }

            if (descriptor.Items == null)
            {
                descriptor.Items = new Dictionary<string, DiskCacheDocumentationDescriptor>(StringComparer.OrdinalIgnoreCase);
                log.LogDebug("Cache descriptor map was missing");
            }

            return descriptor;
        }

        /// <summary>
        ///     Saves cache descriptor into a file
        /// </summary>
        public void Save(string path, ILogger log)
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(path, json, Encoding.UTF8);
                log.LogDebug("File '{0}' has been written", path);
            }
            catch (Exception e)
            {
                log.LogError(0, e, "Failed to write file '{0}'", path);
                throw;
            }
        }

        /// <summary>
        ///     Creates a deep copy of disk cache descriptor
        /// </summary>
        public DiskCacheDescriptor Clone()
        {
            var json = JsonConvert.SerializeObject(this);
            var copy = JsonConvert.DeserializeObject<DiskCacheDescriptor>(json);
            return copy;
        }
        
        #endregion
    }
}