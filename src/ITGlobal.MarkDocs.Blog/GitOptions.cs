using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Git source options
    /// </summary>
    public sealed class GitOptions
    {
        /// <summary>
        ///     Defines fetch URL
        /// </summary>
        [PublicAPI, NotNull]
        public string FetchUrl { get; set; }

        /// <summary>
        ///     A username for fetch URL
        /// </summary>
        [PublicAPI, CanBeNull]
        public string UserName { get; set; }

        /// <summary>
        ///     A password for fetch UTL
        /// </summary>
        [PublicAPI, CanBeNull]
        public string Password { get; set; }

        /// <summary>
        ///     Defines source branch (default is "master")
        /// </summary>
        [PublicAPI, NotNull]
        public string Branch { get; set; } = "master";
    }
}