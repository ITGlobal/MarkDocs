using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ITGlobal.MarkDocs.Extensions;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using System.Linq;
using Lucene.Net.Search.Spell;
using Lucene.Net.Util;

namespace ITGlobal.MarkDocs.Search
{

    internal sealed class IndexDescriptorItem
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
