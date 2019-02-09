using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace ITGlobal.MarkDocs.Format.Impl.Extensions.HighlightJs
{
    internal sealed class HighlightJsWorker
    {
        private readonly HighlightJsStylesheet _stylesheet;
        private readonly HtmlParser _htmlParser;

        private NpmHelper _npm;
        private NodejsHelper _nodejs;
        private ICssStyleSheet _styles;
        private string _renderJs;

        private readonly TaskCompletionSource<object> _initialized = new TaskCompletionSource<object>();
        private ImmutableHashSet<string> _supportedLanguages;

        public HighlightJsWorker(HighlightJsOptions options, IMarkDocsLog log)
        {
            _stylesheet = options.Stylesheet;
            _htmlParser = new HtmlParser();

            if (!Directory.Exists(options.TempDirectory))
            {
                Directory.CreateDirectory(options.TempDirectory);
            }

            Task.Run(Initialize);


            void Initialize()
            {
                try
                {
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

                    DeployResource(options.TempDirectory, "package.json");
                    var listJs = DeployResource(options.TempDirectory, "hljs-list-langs.js");
                    _renderJs = DeployResource(options.TempDirectory, "hljs-render.js");
                    _npm.Install(options.TempDirectory);

                    _supportedLanguages = ImmutableHashSet.CreateRange(_nodejs.Invoke<string[]>(listJs));
                    if (_supportedLanguages.Contains("cs"))
                    {
                        _supportedLanguages = _supportedLanguages.Add("csharp");
                    }

                    if (_stylesheet != HighlightJsStylesheet.None)
                    {
                        var css = LoadResource($"styles.{ResourceAttribute.GetId(_stylesheet)}");
                        var cssParser = new CssParser();
                        _styles = cssParser.ParseStyleSheet(css);
                    }

                    _initialized.TrySetResult(null);
                }
                catch (Exception e)
                {
                    log.Error(e, "Unable to initiaize hljs");
                    _initialized.TrySetException(e);
                    throw;
                }
            }
        }

        public ImmutableHashSet<string> SupportedLanguages
        {
            get
            {
                _initialized.Task.GetAwaiter().GetResult();
                return _supportedLanguages;
            }
        }

        public string Render(string language, string sourceCode)
        {
            _initialized.Task.GetAwaiter().GetResult();

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

        private string DeployResource(string tempDirectory, string name)
        {
            var dir = Path.Combine(tempDirectory);
            Directory.CreateDirectory(dir);

            var filename = Path.Combine(dir, name);
            var source = LoadResource(name);
            File.WriteAllText(filename, source, Encoding.UTF8);

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