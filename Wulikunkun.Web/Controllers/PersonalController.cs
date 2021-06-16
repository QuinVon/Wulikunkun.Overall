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
using Wulikunkun.Web.Models;

namespace Wulikunkun.Web.Controllers
{
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

        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User as ClaimsPrincipal;
            string userId = _signManager.UserManager.GetUserId(claimsPrincipal);
            /* 可以留意一下这里异步ToList的API */
            List<Article> articles = await _dbContext.Articles.Where(article => article.UserId == userId).ToListAsync();
            return View(articles);
        }
    }
}