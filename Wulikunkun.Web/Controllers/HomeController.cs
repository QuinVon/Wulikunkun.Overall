using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using Wulikunkun.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Security.Cryptography;

namespace Wulikunkun.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly WangKunDbContext dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, WangKunDbContext wangKunDbContext)
        {
            this.dbContext = wangKunDbContext;
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

        public JsonResult LogIn(User user)
        {
            User corrUser = dbContext.Users.FirstOrDefault(item => item.Name == user.Name);
            string userSalt = corrUser.Salt;
            var passwordAndSaltBytes = Encoding.UTF8.GetBytes(user.Password + userSalt);
            var hashBytes = new SHA256Managed().ComputeHash(passwordAndSaltBytes);
            var hashString = Convert.ToBase64String(hashBytes);
            HttpContext.Session.SetString("UserName",user.Name);
            if (hashString == corrUser.Password)
            {
                return Json(new
                {
                    StatusCode = 1
                });
            }

            return Json(new
            {
                StatusCode = 0
            });
        }
    }
}