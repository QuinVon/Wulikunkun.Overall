using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Wulikunkun.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Wulikunkun.Web.Controllers
{
    public class LogInController : Controller
    {
        private readonly WangKunDbContext dbContext;
        private readonly ILogger<LogInController> _logger;

        public LogInController(ILogger<LogInController> logger, WangKunDbContext wangKunDbContext)
        {
            this.dbContext = wangKunDbContext;
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
            if (dbContext.Users.Any(item => item.Phone.Equals(user.Phone)))
            {
                result = new
                {
                    Message = "该手机已经注册！",
                    StateCode = 3
                };
                return Json(result);
            }
            string salt = Guid.NewGuid().ToString();
            byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(user.Password + salt);
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordAndSaltBytes);
            string hashString = Convert.ToBase64String(hashBytes);
            user.Password = hashString;
            user.RegisterTime = DateTime.Now;
            user.Salt = salt;
            user.UserRole = Role.CommonUser;
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            result = new
            {
                Message = "注册成功！",
                StateCode = 1
            };
            HttpContext.Session.SetString("username", user.Email);
            JsonResult jsonResult = Json(result);
            return jsonResult;
        }

    }
}