using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.CodeBlockRenderers
{
    internal sealed class HighlightJsWorker
    {
        private readonly HighlightJsStylesheet _stylesheet;
        private readonly HtmlParser _htmlParser;

        private readonly NpmHelper _npm;
        private readonly NodejsHelper _nodejs;
        private readonly ICssStyleSheet _styles;
        private readonly string _renderJs;

        public HighlightJsWorker(HighlightJsOptions options, IMarkDocsLog log)
        {
            _stylesheet = options.Stylesheet;
            _htmlParser = new HtmlParser();

            if (!Directory.Exists(options.TempDirectory))
            {
                Directory.CreateDirectory(options.TempDirectory);
            }

            _nodejs = new NodejsHelper(log);
            if (!_nodejs.IsNodeInstalled)
            {
                throw new Exception("NodeJS executable is not found in PATH");
            }

            _npm = new NpmHelper(log);
            if (!_npm.IsNpmInstalled)
            {
                throw new Exception("NPM executable is not found in PATH");
            }

            var listJs = DeployJs(options.TempDirectory, "hljs-list-langs.js");
            _renderJs = DeployJs(options.TempDirectory, "hljs-render.js");
            SupportedLanguages = ImmutableHashSet.CreateRange(_nodejs.Invoke<string[]>(listJs));

            if (_stylesheet != HighlightJsStylesheet.None)
            {
                var css = LoadResource($"styles.{ResourceAttribute.GetId(_stylesheet)}");
                var cssParser = new CssParser();
                _styles = cssParser.ParseStyleSheet(css);
            }
        }

        public ImmutableHashSet<string> SupportedLanguages { get; }

        public string Render(string language, string sourceCode)
        {
            var html = _nodejs.Invoke<string>(_renderJs, new { language, sourceCode });

            if (_stylesheet != HighlightJsStylesheet.None)
            {
                var doc = _htmlParser.ParseDocument(html);
                InlineCss(doc.Body, _styles);
                html = doc.Body.InnerHtml;
            }

            html = $"<pre><code>{html}</code></pre>";
            return html;
        }

        private static string LoadResource(string name)
        {
            using (var stream = typeof(HighlightJsWorker).GetTypeInfo()
                .Assembly
                .GetManifestResourceStream($"ITGlobal.MarkDocs.Resources.{name}"))
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

        private string DeployJs(string tempDirectory, string name)
        {
            var dir = Path.Combine(tempDirectory);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var packageJson = Path.Combine(dir, "package.json");
            if (!File.Exists(packageJson))
            {
                const string packageJsonContent = @"{
  ""name"": ""ServerHighlightJsSyntaxColorizer"",
  ""version"": ""1.0.0"",
  ""description"": ""none"",
  ""dependencies"": {
    ""highlightjs"": ""^ 9.10.0""
  },
  ""author"": """",
  ""repository"": ""none"",
  ""license"": ""MIT""
}";
                File.WriteAllText(packageJson, packageJsonContent, Encoding.UTF8);
            }

            var filename = Path.Combine(dir, name);
            var source = LoadResource(name);
            File.WriteAllText(filename, source, Encoding.UTF8);

            _npm.Install(dir);

            return filename;
        }

        private static void InlineCss(IHtmlElement element, ICssStyleSheet styles)
        {
            if (element.HasChildNodes)
            {
                foreach (var node in element.ChildNodes)
                {
                    if (node is IHtmlElement e)
                    {
                        InlineCss(e, styles);
                    }
                }
            }

            if (element.GetStyle() == null)
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