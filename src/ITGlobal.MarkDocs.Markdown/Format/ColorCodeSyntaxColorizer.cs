using System;
using System.IO;
using System.Linq;
using ColorCode;
using ColorCode.Formatting;
using ColorCode.Styling.StyleSheets;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A server-side syntax colorizer based on ColorCode library
    /// </summary>
    [PublicAPI]
    public sealed class ColorCodeSyntaxColorizer : ISyntaxColorizer
    {
        private static readonly HtmlFormatter Formatter = new HtmlFormatter();
        private static readonly DefaultStyleSheet StyleSheet = new DefaultStyleSheet();

        /// <summary>
        ///     List of supported languages
        /// </summary>
        public string[] SupportedLanguages { get; } = Languages.All.Select(_ => _.Id).ToArray();

        /// <summary>
        ///     Initializes syntax colorizer
        /// </summary>
        public void Initialize(ILogger log) { }

        /// <summary>
        ///     Render a source code into an HTML
        /// </summary>
        public string Render(string languageId, string sourceCode)
        {
            var language = Languages.FindById(languageId);
            if (language == null)
            {
                return $"<pre><code class=\"{languageId}\">{sourceCode}</code></pre>";
            }

            var colorizer = new CodeColorizer();
            using (var writer = new StringWriter())
            {
                colorizer.Colorize(sourceCode, language, Formatter, StyleSheet, writer);
                var html = writer.ToString();
                return html;
            }
        }
    }
}