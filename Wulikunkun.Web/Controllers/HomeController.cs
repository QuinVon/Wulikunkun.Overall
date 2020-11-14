using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using Wulikunkun.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Wulikunkun.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext ApplicationDbContext, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _dbContext = ApplicationDbContext;
            _logger = logger;
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

        public JsonResult LogIn(ApplicationUser user)
        {
            ApplicationUser corrUser = _userManager.FindByNameAsync(user.UserName).Result;

            if (corrUser == null)
            {
                return Json(new
                {
                    StatusCode = 0
                });
            }

            string userSalt = corrUser.Salt;
            // var passwordAndSaltBytes = Encoding.UTF8.GetBytes(user.Password + userSalt);
            // var hashBytes = new SHA256Managed().ComputeHash(passwordAndSaltBytes);
            // var hashString = Convert.ToBase64String(hashBytes);
            HttpContext.Session.SetString("UserName", user.UserName);
            // if (hashString == corrUser.Password)
            // {
            //     return Json(new
            //     {
            //         StatusCode = 1
            //     });
            // }

            return Json(new
            {
                StatusCode = 0
            });
        }
    }
}