using System.IO;
using ITGlobal.MarkDocs.Cache;

namespace ITGlobal.MarkDocs.StaticGen
{
    public sealed class MinimalBootstrapTemplate : ITemplate
    {
        public string Name => "minimal";

        public void Initialize(ICacheUpdateOperation operation, IDocumentation documentation) { }

        public void Render(IPage page, string content, TextWriter writer)
        {
            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<html lang=\"en\">");
            writer.WriteLine("<head>");
            writer.WriteLine("  <meta charset=\"utf-8\">");
            writer.WriteLine("  <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
            writer.WriteLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            writer.WriteLine($"  <title>{page.Title}</title>");
            writer.WriteLine($"  <link href=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.7/css/bootstrap.min.css\" rel=\"stylesheet\">");
            writer.WriteLine($"  <link href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\" rel=\"stylesheet\">");
            writer.WriteLine("  <!--[if lt IE 9]>");
            writer.WriteLine($"    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/html5shiv/3.7.3/html5shiv.min.js\"></script>");
            writer.WriteLine($"    <script src=\"https://cdnjs.cloudflare.com/ajax/libs/respond.js/1.4.2/respond.min.js\"></script>");
            writer.WriteLine("  <![endif]-->");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            writer.WriteLine("  <div class=\"container\">");
            writer.WriteLine(content);
            writer.WriteLine("  </div>");
            writer.WriteLine($"  <script src=\"https://cdnjs.cloudflare.com/ajax/libs/jquery/3.1.1/jquery.min.js\"></script>");
            writer.WriteLine($"  <script src=\"https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.7/js/bootstrap.min.js\"></script>");
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
        }
    }
}