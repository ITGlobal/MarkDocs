using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ITGlobal.MarkDocs.Extensions;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Search
{

    /// <summary>
    ///     Options for search extension
    /// </summary>
    [PublicAPI]
    public sealed class SearchOptions
    {
        /// <summary>
        ///     Path to lucene index directory
        /// </summary>
        [PublicAPI, NotNull]
        public string IndexDirectory { get; set; }
    }
}
