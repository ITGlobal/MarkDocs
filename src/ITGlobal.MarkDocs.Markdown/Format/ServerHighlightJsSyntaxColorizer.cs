using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AngleSharp;
using AngleSharp.Dom.Css;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Css;
using AngleSharp.Parser.Html;
using AngleSharp.Services;
using JetBrains.Annotations;
using Jint;
using Jint.Native;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A server-side syntax colorizer that uses highlight.js
    /// </summary>
    [PublicAPI]
    public sealed class ServerHighlightJsSyntaxColorizer : ISyntaxColorizer
    {
        private readonly HighlightJsStylesheet _stylesheet;
        private readonly Engine _jintEngine;
        private readonly HtmlParser _htmlParser;

        private ICssStyleSheet _styles;

        static ServerHighlightJsSyntaxColorizer()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public ServerHighlightJsSyntaxColorizer()
            : this(HighlightJsStylesheet.Vs)
        { }

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public ServerHighlightJsSyntaxColorizer(HighlightJsStylesheet stylesheet)
        {
            _stylesheet = stylesheet;
            _jintEngine = new Engine();
            _htmlParser = new HtmlParser();
        }

        /// <summary>
        ///     List of supported languages
        /// </summary>
        public string[] SupportedLanguages { get; private set; }

        /// <summary>
        ///     Initializes syntax colorizer
        /// </summary>
        public void Initialize()
        {
            var js = LoadResource("Resources.highlight.min.js");
            _jintEngine.Execute("var exports = {};");
            _jintEngine.Execute(js);
            _jintEngine.Execute("var hljs = exports;");
            js = LoadResource("Resources.highlight.pack.js");
            _jintEngine.Execute(js);

            _jintEngine.Execute("var langs = hljs.listLanguages();");
            SupportedLanguages = _jintEngine.GetValue("langs").AsArray()
                .GetOwnProperties()
                .Where(_ => _.Value.Value.IsString())
                .Select(_ => _.Value.Value.AsString())
                .ToArray();

            if (_stylesheet != HighlightJsStylesheet.None)
            {
                var css = LoadResource($"Resources.styles.{ResourceAttribute.GetId(_stylesheet)}");
                var cssParser = new CssParser();
                _styles = cssParser.ParseStylesheet(css);
            }
        }

        /// <summary>
        ///     Render a source code into an HTML
        /// </summary>
        public string Render(string language, string sourceCode)
        {
            _jintEngine.SetValue("sourceCode", sourceCode);
            _jintEngine.SetValue("language", language);
            _jintEngine.Execute("var output = hljs.highlight(language, sourceCode, true).value;");
            var html = _jintEngine.GetValue("output").AsString();

            if (_stylesheet != HighlightJsStylesheet.None)
            {
                var doc = _htmlParser.Parse(html);
                InlineCss(doc.Body, _styles);
                html = doc.Body.InnerHtml;
            }

            html = $"<pre><code>{html}</code></pre>";
            return html;
        }

        private static string LoadResource(string name)
        {
            using (var stream = typeof(ServerHighlightJsSyntaxColorizer).GetTypeInfo()
                .Assembly
                .GetManifestResourceStream($"ITGlobal.MarkDocs.Markdown.{name}"))
            {
                if (stream == null)
                {
                    throw new Exception($"Unable to load resource \"{name}\"");
                }

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var source = reader.ReadToEnd();
                    return source;
                }
            }
        }

        private static void InlineCss(IHtmlElement element, ICssStyleSheet styles)
        {
            if (element.HasChildNodes)
            {
                foreach (var node in element.ChildNodes)
                {
                    var e = node as IHtmlElement;
                    if (e != null)
                    {
                        InlineCss(e, styles);
                    }
                }
            }

            if (element.Style == null)
            {
                element.SetAttribute("style", "");
            }

            var props = new List<string>();
            foreach (var rule in styles.Rules.OfType<ICssStyleRule>())
            {
                if (rule.Selector.Match(element))
                {
                    foreach (var p in rule.Style)
                    {
                        if (p.IsImportant)
                        {
                            props.Add($"{p.Name}: {p.Value} !important");
                        }
                        else
                        {
                            props.Add($"{p.Name}: {p.Value}");
                        }
                    }
                }
            }

            if (props.Count > 0)
            {
                element.SetAttribute("style", string.Join("; ", props));
                element.RemoveAttribute("class");
            }
        }
    }
}
