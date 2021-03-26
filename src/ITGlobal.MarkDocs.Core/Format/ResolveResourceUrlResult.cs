using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Result for <see cref="IPageReadContext.ResolveResourceUrl"/>
    /// </summary>
    public sealed class ResolveResourceUrlResult
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public ResolveResourceUrlResult([CanBeNull] string url, [CanBeNull] string anchorToValidate = null)
        {
            Url = url;
            AnchorToValidate = anchorToValidate;
        }

        /// <summary>
        ///     Resolved URL
        /// </summary>
        [CanBeNull]
        public string Url { get; }

        /// <summary>
        ///     An anchor to validate
        /// </summary>
        [CanBeNull]
        public string AnchorToValidate { get; }

        /// <summary>
        ///     Empty instance
        /// </summary>
        [NotNull]
        public static ResolveResourceUrlResult Empty { get; } = new ResolveResourceUrlResult(null, null);

        /// <summary>
        ///     dector
        /// </summary>
        public void Deconstruct([CanBeNull] out string url, [CanBeNull] out string anchorToValidate)
        {
            url = Url;
            anchorToValidate = AnchorToValidate;
        }

    }
}