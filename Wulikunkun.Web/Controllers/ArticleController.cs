using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wulikunkun.Web.Models;

namespace Wulikunkun.Web.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager;


        public ArticleController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signManager = signInManager;
        }
        public IActionResult Editor()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Submit(Article article)
        {
            /* 这是目前暂时采用的获取用户id的方式 */
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string userId = _signManager.UserManager.GetUserId(claimsPrincipal);
            
            article.UserId=userId;
            article.PublishTime = DateTime.Now;
            article.Status = ArticleStatus.NotAllowed;
            _dbContext.Articles.Add(article);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}