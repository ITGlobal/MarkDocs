using System;
using System.IO;
using System.Linq;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using SharpYaml.Serialization;

namespace ITGlobal.MarkDocs.Format
{
    internal sealed class YamlMetadataExtractor : IMetadataExtractor
    {
        public void TryExtract(IParsePropertiesContext ctx, MarkdownDocument document, Metadata metadata)
        {
            var block = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            if (block == null)
            {
                return;
            }

            try
            {
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
            catch (Exception e)
            {
                ctx.Error($"Malformed YAML frontmatter. {e.Message}", block.Line, e);
            }
        }

        private static void ProcessNode(Metadata metadata, string key, YamlNode node)
        {
            switch (key)
            {
                case "id":
                    var id = (node as YamlScalarNode)?.Value ?? "";
                    if (!string.IsNullOrEmpty(id))
                    {
                        metadata.ContentId = id;
                    }
                    break;
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

                case "description":
                    var description = (node as YamlScalarNode)?.Value ?? "";
                    if (!string.IsNullOrEmpty(description))
                    {
                        CombineMetaTags(metadata, "description", description);
                    }
                    break;

                case "meta":
                    var map = node as YamlMappingNode;
                    if (map != null)
                    {
                        var metaTags = map.Children
                            .Select(pair => new
                            {
                                Name = (pair.Key as YamlScalarNode)?.Value,
                                Content = (pair.Value as YamlScalarNode)?.Value
                            })
                            .Where(_ => !string.IsNullOrEmpty(_.Name));
                        foreach (var metaTag in metaTags)
                        {
                            CombineMetaTags(metadata, metaTag.Name, metaTag.Content);
                        }
                    }
                    break;
            }
        }

        private static void CombineMetaTags(Metadata metadata, string name, string content)
        {
            var existing = metadata.MetaTags.FirstOrDefault(_ => _.Name == name);
            if (existing != null)
            {
                existing.Content = content;
            }

            metadata.MetaTags = metadata.MetaTags.Concat(new[] { new MetaTag { Name = name, Content = content } }).ToArray();
        }
    }
}