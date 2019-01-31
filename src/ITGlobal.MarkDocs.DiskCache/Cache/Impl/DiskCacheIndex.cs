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
    // DiskCacheIndex
    //
    // ------------------------------------------------------------------------
    // 
    // Stores information about assets stored in cache, their file names and
    // content hashes.
    //
    // JSON model
    // ----------
    //
    // {
    //    /* "pages" node stores information about page assets */
    //    "pages" : {
    //       "PAGE_ID_1" : {
    //         "filename" : "CACHE_FILE_NAME",
    //         "hash" : "ASSET_HASH"
    //       },
    //       "PAGE_ID_2" : {
    //         "filename" : "CACHE_FILE_NAME",
    //         "hash" : "ASSET_HASH"
    //       },
    //       /* ... */
    //       "PAGE_ID_N" : {
    //         "filename" : "CACHE_FILE_NAME",
    //         "hash" : "ASSET_HASH"
    //       }
    //    },
    //    /* "previews" node stores information about page preview assets */
    //    "previews" : { /* structure is similar to "pages" node */ },
    //    /* "files" node stores information about file attachments */
    //    "files" : { /* structure is similar to "pages" node */ },
    //    /* "generated" node stores information about generated attachments */
    //    "generated" : { /* structure is similar to "pages" node */ }
    // }
    //
    // ------------------------------------------------------------------------
    internal sealed class DiskCacheIndex
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public const string FILE_NAME = "index.json";

        public sealed class Dictionary : Dictionary<string, Item>
        {
            public Dictionary()
                : base(StringComparer.OrdinalIgnoreCase)
            { }

            public void Set(string id, string filename, string hash)
            {
                this[id] = new Item
                {
                    Filename = filename,
                    Hash = hash,
                };
            }
        }

        public sealed class Item
        {
            [JsonProperty("filename")]
            public string Filename { get; set; }

            [JsonProperty("hash")]
            public string Hash { get; set; }
        }

        [JsonProperty("pages")]
        public Dictionary Pages { get; } = new Dictionary();

        [JsonProperty("previews")]
        public Dictionary PagePreviews { get; } = new Dictionary();

        [JsonProperty("files")]
        public Dictionary Files { get; } = new Dictionary();

        [JsonProperty("generated")]
        public Dictionary GeneratedFiles { get; } = new Dictionary();
        
        public static DiskCacheIndex Load(string path, IMarkDocsLog log)
        {
            DiskCacheIndex index;
            if (!File.Exists(path))
            {
                log.Error($"Cache index file '{path}' doesn't exist");
                return null;
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                index = JsonConvert.DeserializeObject<DiskCacheIndex>(json);
            }
            catch (Exception e)
            {
                log.Error(e, $"Unable to load cache index from '{path}'");
                return null;
            }

            if (index == null)
            {
                index = new DiskCacheIndex();
            }

            return index;
        }

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}