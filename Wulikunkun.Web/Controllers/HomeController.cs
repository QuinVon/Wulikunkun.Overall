using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wulikunkun.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Wulikunkun.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext ApplicationDbContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _dbContext = ApplicationDbContext;
            _logger = logger;
            _roleManager = roleManager;
            _signManager = signInManager;
        }


        public IActionResult Index()
        {
            if (HttpContext.Session.IsAvailable)
            {
                string username = HttpContext.Session.GetString("UserName");
                if (username != null)
                {
                    ViewBag.UserName = username;
                }
            }

            return View();
        }

        /* 尽管这里的Action以Async结尾，但是在前端页面发起Ajax请求时URL里面不可以给Action名称加Async，否则请求不到  */
        public async Task<IActionResult> LogInAsync(RegisterUser user)
        {
            /* 判断用户是否存在 */
            ApplicationUser corrUser = _userManager.FindByNameAsync(user.UserName).Result;

            if (corrUser == null)
            {
                return Json(new
                {
                    StatusCode = 0
                });
            }

            /* 如果用户存在，判断密码是否正确 */
            var passwordVerifyResult = await _userManager.CheckPasswordAsync(corrUser, user.Password);
            if (!passwordVerifyResult)
            {
                return Json(new
                {
                    StatusCode = 0
                });
            }
            else
            {
                /* 人家微软官方的Doc其实没问题，这里的await千万不能漏写，否则得到的就是一个Task<IdentityResult>而不是IdentityResult，另外就是需要注意，在使用Identity进行用户认证时，该用户在aspnetusers表中的EmailConfirmed必须为true才会允许对该用户进行验证 */
                var logInResult = await _signManager.PasswordSignInAsync(user.UserName, user.Password, true, true);
                if (logInResult.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,corrUser.UserName),
                        new Claim(ClaimTypes.Email,corrUser.Email),
                        new Claim("UserId",corrUser.Id)
                     };
                    await _userManager.AddClaimsAsync(corrUser, claims);
                }
                return Json(new
                {
                    StatusCode = 1
                });
            }
        }
    }
}