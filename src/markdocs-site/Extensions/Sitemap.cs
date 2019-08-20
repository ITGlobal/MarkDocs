using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ITGlobal.MarkDocs.Site.Extensions
{
    [XmlRoot("urlset", Namespace = Namespace)]
    public sealed class Sitemap
    {
        internal const string Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

        internal static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Sitemap));

        public sealed class Item
        {
            [XmlElement("loc", Namespace = Namespace)]
            public string Location { get; set; }
        }

        [XmlElement("url", Namespace = Namespace)]
        public Item[] Items { get; set; }

        public byte[] ToByteArray()
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(ToByteArray());
        }

        public static Sitemap Create(IDocumentation documentation)
        {
            return new Sitemap
            {
                Items = (
                    from n in EnumeratePages()
                    select new Item
                    {
                        Location = n.GetResourceUrl()
                    }
                ).ToArray()
            };

            IEnumerable<IPage> EnumeratePages(IPage root = null)
            {
                root = root ?? documentation.RootPage;
                yield return root;

                foreach (var nestedPage in root.NestedPages)
                {
                    foreach (var page in EnumeratePages(nestedPage))
                    {
                        yield return page;
                    }
                }
            }
        }
    }
}