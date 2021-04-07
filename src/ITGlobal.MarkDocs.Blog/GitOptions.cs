using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Git source options
    /// </summary>
    [PublicAPI]
    public sealed class GitOptions
    {
        /// <summary>
        ///     Defines fetch URL
        /// </summary>
        [NotNull]
        public string FetchUrl { get; set; }

        /// <summary>
        ///     A username for fetch URL
        /// </summary>
        [CanBeNull]
        public string UserName { get; set; }

        /// <summary>
        ///     A password for fetch UTL
        /// </summary>
        [CanBeNull]
        public string Password { get; set; }

        /// <summary>
        ///     Defines source branch (default is "master")
        /// </summary>
        [NotNull]
        public string Branch { get; set; } = "master";

        /// <summary>
        ///     Enable periodical remote polling
        /// </summary>
        public bool EnablePolling { get; set; } = true;

        /// <summary>
        ///     Remote polling timer interval
        /// </summary>
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(5);
    }
}