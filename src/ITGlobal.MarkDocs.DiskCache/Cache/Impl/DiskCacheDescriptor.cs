using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    // ------------------------------------------------------------------------
    //
    // DiskCacheDescriptor
    //
    // ------------------------------------------------------------------------
    // 
    // Stores information about documentation versions and their cache directories.
    //
    // JSON model
    // ----------
    //
    // {
    //    "items" : {
    //      "DOCUMENTATION_VERSION_ID_1" : {
    //        "directory" : "SUBDIRECTORY_NAME",
    //        "hash" : "DOCUMENTATION_SOURCE_URL"
    //      },
    //      "DOCUMENTATION_VERSION_ID_2" : {
    //        "directory" : "SUBDIRECTORY_NAME",
    //        "hash" : "DOCUMENTATION_SOURCE_URL"
    //      },
    //      /* ... */
    //      "DOCUMENTATION_VERSION_ID_N" : {
    //        "directory" : "SUBDIRECTORY_NAME",
    //        "hash" : "DOCUMENTATION_SOURCE_URL"
    //      }
    //    }
    // }
    //
    // ------------------------------------------------------------------------
    internal sealed class DiskCacheDescriptor
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public const string FILE_NAME = "cache.json";

        public sealed class Item
        {
            [JsonProperty("directory")]
            public string Directory { get; set; }
            [JsonProperty("hash")]
            public string Hash { get; set; }
        }

        [JsonProperty("items")]
        public Dictionary<string, Item> Items { get; }
            = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);

        public void Update(string id, string directory, string hash)
        {
            Items[id] = new Item
            {
                Directory = directory,
                Hash = hash
            };
        }

        /// <summary>
        ///     Loads a cache descriptor from a file or creates a new one
        /// </summary>
        public static DiskCacheDescriptor LoadOrCreate(string path, IMarkDocsLog log)
        {
            DiskCacheDescriptor descriptor = null;

            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path, Encoding.UTF8);
                    descriptor = JsonConvert.DeserializeObject<DiskCacheDescriptor>(json);
                }
                catch (Exception e)
                {
                    log.Error(e, $"Unable to load cache descriptor from '{path}'");
                    descriptor = null;
                }
            }
            else
            {
                log.Warning($"Cache descriptor file '{path}' doesn't exist");
            }

            if (descriptor == null)
            {
                descriptor = new DiskCacheDescriptor();
            }

            return descriptor;
        }

        /// <summary>
        ///     Saves cache descriptor into a file
        /// </summary>
        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}