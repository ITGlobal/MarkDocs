using System;
using System.Linq;
using ITGlobal.MarkDocs.Search;
using ITGlobal.MarkDocs.Tools.Serve.Models;
using Microsoft.AspNetCore.Mvc;

namespace ITGlobal.MarkDocs.Tools.Serve.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly IMarkDocService _markDocs;

        public HomeController(IMarkDocService markDocs)
        {
            _markDocs = markDocs;
        }

        [HttpGet("{*path}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false, VaryByQueryKeys = new[] { "version" })]
        public IActionResult Resource(string path = "", string q = null)
        {
            var documentation = _markDocs.Documentations.FirstOrDefault();
            if (documentation == null)
            {
                throw new Exception("Documentation is not configured");
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                if (!string.IsNullOrWhiteSpace(path) && path != "/")
                {
                    return RedirectToAction("Resource", "Home", new { path });
                }

                SearchPageModel model;
                if (!string.IsNullOrEmpty(q))
                {
                    var searchService = documentation.GetSearchService();
                    var results = searchService.Search(q);
                    var suggestions = searchService.Suggest(q);
                    model = new SearchPageModel(
                        documentation,
                        q,
                        results,
                        suggestions
                    );
                }
                else
                {
                    model = new SearchPageModel(documentation);
                }

                return View("~/Serve/Views/Home/Search.cshtml", model);
            }

            var page = documentation.GetPage(path);
            if (page != null)
            {
                return View("~/Serve/Views/Home/Page.cshtml", page);
            }

            var file = documentation.GetAttachment(path);
            if (file != null)
            {
                return File(file.OpenRead(), file.ContentType);
            }

            return NotFound(new { Error = "Not found" });
        }

        [HttpGet("sitemap.xml")]
        public ActionResult Sitemap()
        {
            var documentation = _markDocs.Documentations.FirstOrDefault();
            if (documentation == null)
            {
                throw new Exception("Documentation is not configured");
            }

            var sitemap = Extensions.Sitemap.Create(documentation);
            return Content(sitemap.ToString(), "text/xml");
        }
    }
}
