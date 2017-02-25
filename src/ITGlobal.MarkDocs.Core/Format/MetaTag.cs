using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Meta tag
    /// </summary>
    [PublicAPI]
    public sealed class MetaTag
    {
        /// <summary>
        ///     Tag name
        /// </summary>
        [PublicAPI, NotNull]
        public string Name { get; set; }

        /// <summary>
        ///     Tag content
        /// </summary>
        [PublicAPI, NotNull]
        public string Content { get; set; }
    }
}