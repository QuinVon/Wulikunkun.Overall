using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wulikunkun.Web.Models;

namespace Wulikunkun.Web.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ArticleController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Editor()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Submit(string markdownDoc, string htmlCode)
        {
            var claimIdentity = User.Identity as ClaimsIdentity;
            string userId = claimIdentity.FindFirst("UserId").Value;
            Article article = new Article
            {
                MarkContent = markdownDoc,
                HtmlContent = htmlCode
            };
            _dbContext.Articles.Add(article);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}