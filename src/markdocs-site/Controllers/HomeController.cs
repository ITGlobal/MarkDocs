using System;
using System.Linq;
using ITGlobal.MarkDocs.Search;
using ITGlobal.MarkDocs.Site.Extensions;
using ITGlobal.MarkDocs.Site.Models;
using Microsoft.AspNetCore.Mvc;

namespace ITGlobal.MarkDocs.Site.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly IMarkDocService _markDocs;

        public HomeController(IMarkDocService markDocs)
        {
            _markDocs = markDocs;
        }

        [HttpGet("{*path}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false, VaryByQueryKeys = new []{"version"})]
        public IActionResult Resource(string path = "")
        {
            var documentation = _markDocs.Documentations.FirstOrDefault();
            if (documentation == null)
            {
                throw new Exception("Documentation is not configured");
            }

            var page = documentation.GetPage(path);
            if (page != null)
            {
                return View("Page", page);
            }

            var file = documentation.GetAttachment(path);
            if (file != null)
            {
                return File(file.OpenRead(), file.ContentType);
            }

            return NotFound(new { Error = "Not found" });
        }

        [HttpGet("_/search")]
        [HttpPost("_/search")]
        public IActionResult Search(string q = null)
        {
            var documentation = _markDocs.Documentations.FirstOrDefault();
            if (documentation == null)
            {
                throw new Exception("Documentation is not configured");
            }

            if (!string.IsNullOrEmpty(q))
            {
                var searchService = documentation.GetSearchService();
                var results = searchService.Search(q);
                var suggestions = searchService.Suggest(q);
                return View(new SearchPageModel(
                    documentation,
                    q,
                    results,
                    suggestions
                ));
            }

            return View(new SearchPageModel(documentation));
        }

        [HttpGet("sitemap.xml")]
        public ActionResult GetSitemap()
        {
            var documentation = _markDocs.Documentations.FirstOrDefault();
            if (documentation == null)
            {
                throw new Exception("Documentation is not configured");
            }

            var sitemap = Sitemap.Create(documentation);
            return Content(sitemap.ToString(), "text/xml");
        }
    }
}
