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
    }
}