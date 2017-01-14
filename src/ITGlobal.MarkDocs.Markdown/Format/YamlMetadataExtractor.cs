using System.IO;
using System.Linq;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using SharpYaml.Serialization;

namespace ITGlobal.MarkDocs.Format
{
    internal sealed class YamlMetadataExtractor : IMetadataExtractor
    {
        public void TryExtract(MarkdownDocument document, Metadata metadata)
        {
            var block = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            if (block == null)
            {
                return;
            }

            var yaml = string.Join("\n", from l in block.Lines.Lines select l.Slice.ToString());
            using (var reader = new StringReader(yaml))
            {
                var stream = new YamlStream();
                stream.Load(reader);

                var properties = stream.Documents[0].RootNode as YamlMappingNode;
                if (properties != null)
                {
                    foreach (var pair in properties.Children)
                    {
                        var key = (pair.Key as YamlScalarNode)?.Value;
                        if (key != null)
                        {
                            ProcessNode(metadata, key, pair.Value);
                        }
                    }
                }
            }
        }

        private static void ProcessNode(Metadata metadata, string key, YamlNode node)
        {
            switch (key)
            {
                case "title":
                    var title = (node as YamlScalarNode)?.Value ?? "";
                    if (!string.IsNullOrEmpty(title))
                    {
                        metadata.Title = title;
                    }
                    break;
                case "order":
                    var raw = (node as YamlScalarNode)?.Value ?? "";
                    int order;
                    if (int.TryParse(raw, out order))
                    {
                        metadata.Order = order;
                    }
                    break;

                case "tags":
                    var list = node as YamlSequenceNode;
                    if (list != null)
                    {
                        var tags = list.Children.OfType<YamlScalarNode>().Select(_ => _.Value).ToArray();
                        metadata.Tags = tags;
                    }
                    break;
            }
        }
    }
}