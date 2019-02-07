using System;
using ITGlobal.MarkDocs.Blog.Example.Models;
using Microsoft.AspNetCore.Mvc;

namespace ITGlobal.MarkDocs.Blog.Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogEngine _engine;

        public HomeController(IBlogEngine engine)
        {
            _engine = engine;
        }

        [HttpGet("")]
        public IActionResult List(int page = 1)
        {
            var posts = _engine.Index.List(page - 1);
            return View("List", new ListModel
            {
                Page = page,
                TotalPages = (int)Math.Ceiling(_engine.Index.Count * 1f / BlogEngineConstants.PageSize),
                Posts = posts
            });
        }

        [HttpGet("{year}/{month}")]
        public IActionResult Month(int year, int month, int page = 1)
        {
            var posts = _engine.Index[year][month];
            return View("Month", new MonthModel
            {
                Year = year,
                Month = month,
                Page = page,
                TotalPages = (int)Math.Ceiling(posts.Count * 1f / BlogEngineConstants.PageSize),
                Posts = posts.List(page - 1)
            });
        }

        [HttpGet("{*path}")]
        public IActionResult Resource(string path = "/")
        {
            var resource = _engine.GetResource(path);

            switch (resource)
            {
                case IBlogPost post:
                    return View("Post", post);

                case IBlogPermalink permalink:
                    return Redirect(permalink.Post.Id);

                case IBlogAttachment file:
                    return File(file.OpenRead(), file.ContentType);

                case null:
                    return View("404");

                default:
                    throw new Exception($"Unknown resource type: {resource.GetType()}");
            }
        }

        [HttpGet, Route("tags")]
        public IActionResult Tags()
        {
            var tags = _engine.Index.Tags;
            return View("Tags", tags);
        }

        [HttpGet, Route("tags/{*name}")]
        public IActionResult ByTag(string name, int page = 1)
        {
            var tag = _engine.Index.Tag(name);
            var posts = tag.List(page - 1);
            return View("Tag", new TagModel
            {
                Tag = tag,
                Page = page - 1,
                TotalPages = (int)Math.Ceiling(_engine.Index.Count * 1f / BlogEngineConstants.PageSize),
                Posts = posts
            });
        }

        [HttpGet, Route("search/suggest")]
        public IActionResult Suggest(string q)
        {
            var items = _engine.Suggest(q);
            return Json(new { items });
        }

        [HttpGet, Route("search")]
        public IActionResult Search(string q, int page = 1)
        {
            var result = _engine.Search(q);
            return View("Search", new SearchModel
            {
                Query = result.Query,
                Page = page,
                TotalPages = (int)Math.Ceiling(result.Count * 1f / BlogEngineConstants.PageSize),
                Items = result.List(page - 1)
            });
        }
    }
}
