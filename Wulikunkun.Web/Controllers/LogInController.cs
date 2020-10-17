using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wulikunkun.Web.Models;

namespace Wulikunkun.Web.Controllers
{
    public class LogInController : Controller
    {
        private readonly ILogger<LogInController> _logger;
        private readonly WangKunDbContext dbContext;

        public LogInController(ILogger<LogInController> logger, WangKunDbContext wangKunDbContext)
        {
            dbContext = wangKunDbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult CreateUser(User user)
        {
            object result = null;

            if (dbContext.Users.Any(item => item.Email.Equals(user.Email)))
            {
                result = new
                {
                    Message = "该邮箱已经注册！",
                    StateCode = 2
                };
                return Json(result);
            }
            else if (dbContext.Users.Any(item => item.Name == user.Name))
            {
                return Json(new {StatusCode = 3});
            }

            var salt = Guid.NewGuid().ToString();
            var passwordAndSaltBytes = Encoding.UTF8.GetBytes(user.Password + salt);
            var hashBytes = new SHA256Managed().ComputeHash(passwordAndSaltBytes);
            var hashString = Convert.ToBase64String(hashBytes);
            user.Password = hashString;
            user.RegisterTime = DateTime.Now;
            user.Salt = salt;
            user.UserRole = Role.CommonUser;
            user.IsActive = false;
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            result = new
            {
                Message = "注册成功！",
                StateCode = 1
            };
            HttpContext.Session.SetString("username", user.Email);
            var jsonResult = Json(result);
            return jsonResult;
        }
    }
}