using System;
using System.Collections.Generic;
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
                    SetStringMetadata(nameof(Metadata.ContentId), node, metadata);
                    break;
                case "title":
                    SetStringMetadata(nameof(Metadata.Title), node, metadata);
                    break;
                case "author":
                    SetStringMetadata(nameof(Metadata.LastChangedBy), node, metadata);
                    break;
                case "order":
                    SetIntegerMetadata(nameof(Metadata.Order), node, metadata);
                    break;
                case "tags":
                    SetArrayMetadata(nameof(Metadata.Tags), node, metadata, merge: true);
                    break;
                case "description":
                    SetStringMetadata(node, str =>
                    {
                        metadata.SetString(nameof(Metadata.Description), str);
                    });
                    break;

                case "meta":
                    SetArrayMetadata(nameof(Metadata.MetaTags), node, metadata, merge: true);
                    break;

                default:
                    switch (node)
                    {
                        case YamlScalarNode _:
                            SetStringMetadata(key, node, metadata);
                            break;

                        case YamlSequenceNode _:
                            SetArrayMetadata(key, node, metadata, merge: true);
                            break;

                        case YamlMappingNode _:
                            SetDictionaryMetadata(key, node, metadata, merge: true);
                            break;
                    }
                    break;
            }
        }

        private static void SetStringMetadata(string key, YamlNode node, Metadata metadata)
        {
            switch (node)
            {
                case YamlScalarNode scalar:
                    if (!string.IsNullOrEmpty(scalar.Value))
                    {
                        metadata.SetString(key, scalar.Value);
                    }
                    break;
            }
        }

        private static void SetStringMetadata(YamlNode node, Action<string> func)
        {
            switch (node)
            {
                case YamlScalarNode scalar:
                    if (!string.IsNullOrEmpty(scalar.Value))
                    {
                        func(scalar.Value);
                    }
                    break;
            }
        }

        private static void SetIntegerMetadata(string key, YamlNode node, Metadata metadata)
        {
            switch (node)
            {
                case YamlScalarNode scalar:
                    if (!string.IsNullOrEmpty(scalar.Value) && int.TryParse(scalar.Value, out var value))
                    {
                        metadata.SetInteger(key, value);
                    }
                    break;
            }
        }

        private static void SetArrayMetadata(string key, YamlNode node, Metadata metadata, bool merge)
        {
            switch (node)
            {
                case YamlSequenceNode sequence:
                    var values = sequence.Children.OfType<YamlScalarNode>().Select(_ => _.Value).ToArray();
                    if (values.Length > 0)
                    {
                        metadata.SetArray(key, values, merge);
                    }
                    break;
            }
        }

        private static void SetDictionaryMetadata(string key, YamlNode node, Metadata metadata, bool merge)
        {
            switch (node)
            {
                case YamlMappingNode mapping:
                    {
                        var dict = new Dictionary<string, string>();
                        foreach (var pair in mapping.Children)
                        {
                            var pairKey = (pair.Key as YamlScalarNode)?.Value;
                            var pairValue = (pair.Value as YamlScalarNode)?.Value;
                            if (!string.IsNullOrEmpty(pairKey) &&
                                !string.IsNullOrEmpty(pairValue))
                            {
                                dict[pairKey] = pairValue;
                            }
                        }

                        if (dict.Count > 0)
                        {
                            metadata.SetDictionary(key, dict, merge);
                        }
                    }
                    break;

            }
        }
    }
}