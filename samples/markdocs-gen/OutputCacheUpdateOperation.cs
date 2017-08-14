using System;
using System.IO;
using System.Text;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.StaticGen
{
    internal sealed class OutputCacheUpdateOperation : ICacheUpdateOperation
    {
        private readonly string _directory;
        private readonly ITemplate _template;

        public OutputCacheUpdateOperation(string directory, ITemplate template)
        {
            _directory = directory;
            _template = template;
        }

        public void Clear(IDocumentation documentation)
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }

            Directory.CreateDirectory(_directory);

            _template.Initialize(this, documentation);
        }

        public void Write(IResource item, IResourceContent content, Action callback)
        {
            try
            {
                var filename = OutputCache.GetResourcePath(item);
                if (filename.StartsWith("/"))
                {
                    filename = filename.Substring(1);
                }
                filename = Path.Combine(_directory, filename);
                var directory = Path.GetDirectoryName(filename);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }


                if (item.Type == ResourceType.Page)
                {
                    var page = item.Documentation.GetPage(item.Id);
                    if (page != null)
                    {
                        string html;
                        using (var stream = content.GetContent())
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            html = reader.ReadToEnd();
                        }

                        using (var stream = File.OpenWrite(filename))
                        using (var writer = new StreamWriter(stream))
                        {
                            _template.Render(page, html, writer);
                            return;
                        }
                    }
                }

                using (var fileStream = File.OpenWrite(filename))
                using (var contentStream = content.GetContent())
                {
                    contentStream.CopyTo(fileStream);
                }
            }
            finally
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }

        public void Flush()
        { }

        public void Commit()
        { }

        public void Dispose()
        { }
    }
}