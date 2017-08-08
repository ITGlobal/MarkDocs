using System.IO;
using ITGlobal.MarkDocs.Example.Model;
using ITGlobal.MarkDocs.Tags;
using Microsoft.AspNetCore.Mvc;

namespace ITGlobal.MarkDocs.Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMarkDocService _service;

        public HomeController(IMarkDocService service)
        {
            _service = service;
        }

        [HttpGet, Route("")]
        public IActionResult Index() => View("Index", _service.Documentations);

        [HttpGet, Route("{branch}")]
        public IActionResult RootResource(string branch) => Resource(branch, "");

        [HttpGet, Route("{branch}/{*id}")]
        public IActionResult Resource(string branch, string id)
        {
            if (id.Length > 0 && (id[id.Length - 1] == '/' || id[id.Length - 1] == '\\'))
            {
                return RedirectToAction("Resource", new { branch, id = id.Substring(0, id.Length - 1) });
            }

            var documentation = _service.GetDocumentation(branch);
            if (documentation == null)
            {
                return NotFound($"ResourceNotFound: [{branch}]");
            }

            var page = documentation.GetPage(id);
            if (page != null)
            {
                return View("Page", new PageModel(documentation, page));
            }

            var file = documentation.GetAttachment(id);
            if (file != null)
            {
                return File(file.Read(), file.ContentType);
            }

            return NotFound($"ResourceNotFound: [{branch}!{id}]");
        }

        [HttpPost, Route("refresh")]
        public IActionResult Refresh()
        {
            _service.Synchronize();

            return Redirect("/");
        }
        
        [HttpGet, Route("{branch}/by-tag/{*tag}")]
        public IActionResult Tags(string branch, string tag)
        {
            var documentation = _service.GetDocumentation(branch);
            if (documentation == null)
            {
                return NotFound($"ResourceNotFound: [{branch}]");
            }

            var pages = documentation.GetPagesByTag(tag);

           return View(new PagesByTagModel(documentation, tag, pages));
        }
    }
}
