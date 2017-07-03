using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Css;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Css;
using AngleSharp.Parser.Html;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A server-side syntax colorizer that uses highlight.js
    /// </summary>
    [PublicAPI]
    public sealed class ServerHighlightJsSyntaxColorizer : ISyntaxColorizer
    {
        private readonly HighlightJsStylesheet _stylesheet;
        private readonly string _tempDirectory;
        private readonly HtmlParser _htmlParser;

        private NpmHelper _npm;
        private NodejsHelper _nodejs;
        private ICssStyleSheet _styles;
        private string _renderJs;

        static ServerHighlightJsSyntaxColorizer()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        ///     .ctor
        /// </summary>
        [PublicAPI]
        public ServerHighlightJsSyntaxColorizer(
            string tempDirectory,
            HighlightJsStylesheet stylesheet = HighlightJsStylesheet.Vs)
        {
            _tempDirectory = tempDirectory;
            _stylesheet = stylesheet;
            _htmlParser = new HtmlParser();
        }

        /// <summary>
        ///     List of supported languages
        /// </summary>
        public string[] SupportedLanguages { get; private set; }

        /// <summary>
        ///     Initializes syntax colorizer
        /// </summary>
        public void Initialize(ILogger log)
        {
            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }

            _nodejs = new NodejsHelper(log);
            if (!_nodejs.IsNodeInstalled)
            {
                throw new Exception("nodejs executable is not found in PATH");
            }

            _npm = new NpmHelper(log);
            if (!_npm.IsNpmInstalled)
            {
                throw new Exception("NPM executable is not found in PATH");
            }

            var listJs = DeployJs("hljs-list-langs.js");
            _renderJs = DeployJs("hljs-render.js");
            SupportedLanguages = _nodejs.Invoke<string[]>(listJs);
                
            if (_stylesheet != HighlightJsStylesheet.None)
            {
                var css = LoadResource($"styles.{ResourceAttribute.GetId(_stylesheet)}");
                var cssParser = new CssParser();
                _styles = cssParser.ParseStylesheet(css);
            }
        }

        /// <summary>
        ///     Render a source code into an HTML
        /// </summary>
        public string Render(string language, string sourceCode)
        {
            var html = _nodejs.Invoke<string>(_renderJs, new {language, sourceCode });
              
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
                .GetManifestResourceStream($"ITGlobal.MarkDocs.Markdown.Resources.{name}"))
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

        private string DeployJs(string name)
        {
            var dir = Path.Combine(_tempDirectory);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var filename = Path.Combine(dir, name);
            var source = LoadResource(name);
            File.WriteAllText(filename, source, Encoding.UTF8);

            _npm.Install(dir, "highlightjs");

            return filename;
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
