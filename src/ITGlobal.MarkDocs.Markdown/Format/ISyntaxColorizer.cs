using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A syntax colorizer
    /// </summary>
    [PublicAPI]
    public interface ISyntaxColorizer
    {
        /// <summary>
        ///     List of supported languages
        /// </summary>
        [PublicAPI, NotNull]
        string[] SupportedLanguages { get; }

        /// <summary>
        ///     Initializes syntax colorizer
        /// </summary>
        [PublicAPI]
        void Initialize();

        /// <summary>
        ///     Render a source code into an HTML
        /// </summary>
        [PublicAPI, NotNull]
        string Render([NotNull] string language, [NotNull] string sourceCode);
    }
}