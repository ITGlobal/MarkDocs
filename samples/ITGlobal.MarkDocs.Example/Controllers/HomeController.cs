using System.Collections.Generic;
using System.IO;
using ITGlobal.MarkDocs.Comments;
using ITGlobal.MarkDocs.Example.Model;
using ITGlobal.MarkDocs.Search;
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
            _service.RefreshAllDocumentations();

            return Redirect("/");
        }

        [HttpPost, Route("refresh/{branch}")]
        public IActionResult Refresh(string branch)
        {
            _service.RefreshDocumentation(branch);

            return Redirect($"/{branch}");
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

        [HttpPost, Route("_/comment/add")]
        public IActionResult AddComment(string branch, string page, string user, string text)
        {
            var documentation = _service.GetDocumentation(branch);
            if (documentation == null)
            {
                return NotFound($"ResourceNotFound: [{branch}]");
            }

            var p = documentation.GetPage(page ?? "");
            if (p == null)
            {
                return NotFound($"ResourceNotFound: [{branch}!{page}]");
            }

            var comments = p.GetComments();
            comments.AddComment(user, text);

            return Redirect($"/{branch}/{page}");
        }

        [HttpPost, Route("_/comment/edit")]
        public IActionResult EditComment(string branch, string page, string id, string text)
        {
            var documentation = _service.GetDocumentation(branch);
            if (documentation == null)
            {
                return NotFound($"ResourceNotFound: [{branch}]");
            }

            var p = documentation.GetPage(page ?? "");
            if (p == null)
            {
                return NotFound($"ResourceNotFound: [{branch}!{page}]");
            }

            var comments = p.GetComments();
            var comment = comments.FindComment(id);
            if (comment == null)
            {
                return NotFound($"ResourceNotFound: [{branch}!{page}#{id}]");
            }

            comment.Edit(text);

            return Redirect($"/{branch}/{page}");
        }

        [HttpPost, Route("_/comment/delete")]
        public IActionResult DeleteComment(string branch, string page, string id)
        {
            var documentation = _service.GetDocumentation(branch);
            if (documentation == null)
            {
                return NotFound($"ResourceNotFound: [{branch}]");
            }

            var p = documentation.GetPage(page ?? "");
            if (p == null)
            {
                return NotFound($"ResourceNotFound: [{branch}!{page}]");
            }

            var comments = p.GetComments();
            var comment = comments.FindComment(id);
            if (comment == null)
            {
                return NotFound($"ResourceNotFound: [{branch}!{page}#{id}]");
            }

            comment.Delete();

            return Redirect($"/{branch}/{page}");
        }

        [HttpPost, Route("_/comment/reply")]
        public IActionResult AddReply(string branch, string page, string replyto, string user, string text)
        {
            var documentation = _service.GetDocumentation(branch);
            if (documentation == null)
            {
                return NotFound($"ResourceNotFound: [{branch}]");
            }

            var p = documentation.GetPage(page ?? "");
            if (p == null)
            {
                return NotFound($"ResourceNotFound: [{branch}!{page}]");
            }

            var comments = p.GetComments();
            var comment = comments.FindComment(replyto);
            if (comment == null)
            {
                return NotFound($"ResourceNotFound: [{branch}!{page}#{replyto}]");
            }

            comment.Reply(user, text);
            return Redirect($"/{branch}/{page}");
        }
        
        [HttpGet, Route("_/search/suggestions/{branch}")]
        public IActionResult Suggest(string branch, string q)
        {
            var documentation = _service.GetDocumentation(branch);
            if (documentation == null)
            {
                return NotFound($"ResourceNotFound: [{branch}]");
            }

            var items = documentation.Suggest(q);
            return Json(new { items });
        }

        [HttpGet, Route("_/search/{branch}")]
        public IActionResult Search(string branch, string q)
        {
            var documentation = _service.GetDocumentation(branch);
            if (documentation == null)
            {
                return NotFound($"ResourceNotFound: [{branch}]");
            }

            var results = documentation.Search(q);
            return View(new SearchResultsModel(documentation, results, q));
        }
    }
}
