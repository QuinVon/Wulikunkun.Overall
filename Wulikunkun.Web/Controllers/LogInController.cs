using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wulikunkun.Web.Models;
using StackExchange.Redis;
using Wulikunkun.Utility;

namespace Wulikunkun.Web.Controllers
{
    public class LogInController : Controller
    {
        private readonly ILogger<LogInController> _logger;
        private readonly WangKunDbContext dbContext;
        private static readonly ConnectionMultiplexer _multiplexer = ConnectionMultiplexer.Connect("localhost");
        private static readonly IDatabase _redisDatabase = _multiplexer.GetDatabase();


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
                    StateCode = 2
                };
                return Json(result);
            }
            else if (dbContext.Users.Any(item => item.Name.Equals(user.Name)))
            {
                return Json(new { StateCode = 3 });
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

            _redisDatabase.StringSet(user.Name, salt);
            _redisDatabase.KeyExpire(user.Name, TimeSpan.FromMinutes(2));
            SendEmail.Send(user.Email, "激活邮件", $"请点击下面的链接激活您的账户:<br/>https://www.wulikunkun.com/LogIn/Verify?UserName={user.Name}&ActiveCode={salt}<a href='https://www.wulikunkun.com/LogIn/Verify?UserName={user.Name}&ActiveCode={salt}'></a>");

            result = new
            {
                StateCode = 1
            };
            HttpContext.Session.SetString("username", user.Email);
            var jsonResult = Json(result);
            return jsonResult;
        }

        public ViewResult Verify(string userName)
        {
            return View();
        }
    }
}