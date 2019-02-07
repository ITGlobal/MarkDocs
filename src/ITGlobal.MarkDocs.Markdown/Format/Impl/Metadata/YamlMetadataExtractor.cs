using ITGlobal.MarkDocs.Source;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using SharpYaml.Serialization;
using System;
using System.IO;
using System.Linq;

namespace ITGlobal.MarkDocs.Format.Impl.Metadata
{
    internal sealed class YamlMetadataExtractor : IMetadataExtractor
    {
        public PageMetadata Extract(IPageReadContext ctx, MarkdownDocument document)
        {
            var block = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            if (block == null)
            {
                return PageMetadata.Empty;
            }

            try
            {
                var properties = PageMetadata.Empty;

                var yaml = string.Join("\n", from l in block.Lines.Lines select l.Slice.ToString());
                using (var reader = new StringReader(yaml))
                {
                    var stream = new YamlStream();
                    stream.Load(reader);

                    if (stream.Documents[0].RootNode is YamlMappingNode n)
                    {
                        foreach (var (yamlNode, value) in n.Children)
                        {
                            var key = (yamlNode as YamlScalarNode)?.Value;
                            if (key != null)
                            {
                                ProcessNode(key, value, ref properties);
                            }
                        }
                    }
                }

                return properties;
            }
            catch (Exception e)
            {
                ctx.Error($"Malformed YAML front matter. {e.Message}", block.Line);
                return PageMetadata.Empty;
            }
        }

        private static void ProcessNode(string key, YamlNode node, ref PageMetadata metadata)
        {
            switch (key)
            {
                case "id":
                    {
                        if (GetStringMetadata(node, out var value))
                        {
                            metadata = metadata.WithContentId(value);
                        }
                    }
                    break;

                case "title":
                    {
                        if (GetStringMetadata(node, out var value))
                        {
                            metadata = metadata.WithTitle(value);
                        }
                    }
                    break;

                case "author":
                    {
                        if (GetStringMetadata(node, out var value))
                        {
                            metadata = metadata.WithLastChangedBy(value);
                        }
                    }
                    break;

                case "order":
                    {
                        if (GetIntMetadata(node, out var value))
                        {
                            metadata = metadata.WithOrder(value);
                        }
                    }
                    break;

                case "tags":
                    {
                        if (GetStringArrayMetadata(node, out var values))
                        {
                            metadata = metadata.WithTags(values);
                        }
                    }
                    break;

                case "description":
                    {
                        if (GetStringMetadata(node, out var value))
                        {
                            metadata = metadata.WithDescription(value);
                        }
                    }
                    break;

                case "meta":
                    {
                        if (GetStringArrayMetadata(node, out var values))
                        {
                            metadata = metadata.WithMetaTags(values);
                        }
                    }
                    break;

                default:
                    {
                        if (GetStringMetadata(node, out var value))
                        {
                            metadata = metadata.With(key, value);
                        }
                    }
                    break;
            }
        }

        private static bool GetStringMetadata(YamlNode node, out string value)
        {
            switch (node)
            {
                case YamlScalarNode scalar:
                    if (!string.IsNullOrEmpty(scalar.Value))
                    {
                        value = scalar.Value;
                        return true;
                    }
                    break;
            }

            value = null;
            return false;
        }

        private static bool GetIntMetadata(YamlNode node, out int value)
        {
            switch (node)
            {
                case YamlScalarNode scalar:
                    if (!string.IsNullOrEmpty(scalar.Value) && int.TryParse(scalar.Value, out value))
                    {
                        return true;
                    }
                    break;
            }

            value = 0;
            return false;
        }

        private static bool GetStringArrayMetadata(YamlNode node, out string[] values)
        {
            switch (node)
            {
                case YamlSequenceNode sequence:
                    values = sequence.Children.OfType<YamlScalarNode>().Select(_ => _.Value).ToArray();
                    if (values.Length > 0)
                    {
                        return true;
                    }
                    break;
            }

            values = null;
            return false;
        }
    }
}