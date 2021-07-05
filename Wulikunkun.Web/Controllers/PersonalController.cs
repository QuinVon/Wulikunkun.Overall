using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Models;
using Wulikunkun.Web.Models;

namespace Wulikunkun.Web.Controllers
{
    [Authorize]
    public class PersonalController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<PersonalController> _logger;
        private readonly SignInManager<ApplicationUser> _signManager;


        public PersonalController(ApplicationDbContext dbContext, ILogger<PersonalController> logger, SignInManager<ApplicationUser> signInManager)
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._signManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> GetPartialView(int pageNumber = 1, int pageSize = 10)
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string userId = _signManager.UserManager.GetUserId(claimsPrincipal);
            /* 可以留意一下这里异步ToList的API */
            IQueryable<Article> articles = _dbContext.Articles.Where(article => article.UserId == userId && article.Status != ArticleStatus.Deleted);
            PaginatedList<Article> paginatedArticles = await PaginatedList<Article>.CreateAsync(articles.AsNoTracking(), pageNumber, pageSize);
            return PartialView("_PersonalArticles", paginatedArticles);
        }

        public async Task<IActionResult> DeleteArticle(int? deleteArticleId)
        {
            if (deleteArticleId == null)
            {
                return BadRequest();
            }
            Article targetArticle = await _dbContext.Articles.FindAsync(deleteArticleId);
            targetArticle.Status = ArticleStatus.Deleted;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}