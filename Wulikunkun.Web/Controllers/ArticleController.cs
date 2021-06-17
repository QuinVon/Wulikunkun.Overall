using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wulikunkun.Web.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using Web.Models;

namespace Wulikunkun.Web.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly ILogger<ArticleController> _logger;


        public ArticleController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<ArticleController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signManager = signInManager;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> EditorAsync(int? articleId)
        {
            if (articleId != null)
            {
                var article = await _dbContext.Articles.FindAsync(articleId);
                ViewBag.ArticleTitle = article.Title;
                ViewBag.ArticleCategoryId = article.CategoryId;
                ViewBag.ArticleId = articleId;
                ViewBag.ArticleMarkdownContent = article.MarkContent;
            }
            var categories = await _dbContext.Categories.ToListAsync();
            ViewBag.Categories = categories;
            return View();
        }

        public IActionResult DetailAsync(int articleId)
        {
            ValueTask<Article> articleTask = _dbContext.Articles.FindAsync(articleId);
            Article targetArticle = articleTask.Result;
            ViewBag.ArticleHtmlContent = targetArticle.HtmlContent;
            targetArticle.ViewTimes++;
            _dbContext.SaveChangesAsync();
            return View();
        }


        /* 如果传进来的Article的id不为null，表示修改文章，如果为null，表示新建文章 */
        [Authorize]
        public async Task<IActionResult> SubmitAsync(Article article)
        {
            if (article == null)
            {
                return Json(new ResponseModel("参数有误", (int)HttpStatusCode.BadRequest, null));
            }

            try
            {
                if (article.Id != null)
                {
                    var targetArticle = await _dbContext.Articles.FindAsync(article.Id);
                    targetArticle.CategoryId = article.CategoryId;
                    targetArticle.Title = article.Title;
                    targetArticle.MarkContent = article.MarkContent;
                    targetArticle.HtmlContent = article.HtmlContent;
                    targetArticle.Status = ArticleStatus.Audit;
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    /* 这是目前暂时采用的获取用户id的方式 */
                    ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
                    string userId = _signManager.UserManager.GetUserId(claimsPrincipal);

                    article.UserId = userId;
                    article.PublishTime = DateTime.Now;
                    article.Status = ArticleStatus.Audit;
                    _dbContext.Articles.Add(article);
                    await _dbContext.SaveChangesAsync();
                }
                return Json(new ResponseModel("提交成功", (int)HttpStatusCode.OK, null));
            }
            catch (Exception ex)
            {
                _logger.LogError($"文章创建或者更新失败，失败原因为:{ex.Message}，失败堆栈为:{ex.StackTrace}");
                return Json(new ResponseModel("提交失败", (int)HttpStatusCode.InternalServerError, null));
            }
        }
    }
}