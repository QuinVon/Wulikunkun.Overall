using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Wulikunkun.Utility;
using Wulikunkun.Web.Models;

namespace Wulikunkun.Web.Controllers
{
    public class LogInController : Controller
    {
        private readonly ILogger<LogInController> _logger;
        private readonly ApplicationDbContext dbContext;
        private static readonly ConnectionMultiplexer _multiplexer = ConnectionMultiplexer.Connect($"localhost:6379,password={ Environment.GetEnvironmentVariable("RedisPassword")}");
        private static readonly IDatabase _redisDatabase = _multiplexer.GetDatabase();

        public LogInController(ILogger<LogInController> logger, ApplicationDbContext ApplicationDbContext)
        {
            dbContext = ApplicationDbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult CreateUserAsync(ApplicationUser user)
        {
            if (dbContext.Users.Any(item => item.Email.Equals(user.Email)))
                return Json(new { StateCode = 2 });
            else if (dbContext.Users.Any(item => item.UserName.Equals(user.UserName)))
                return Json(new { StateCode = 3 });

            string salt = Guid.NewGuid().ToString();
            byte[] passwordAndSaltBytes = Encoding.UTF8.GetBytes(user.Password + salt);
            byte[] hashBytes = new SHA256Managed().ComputeHash(passwordAndSaltBytes);
            string hashString = Convert.ToBase64String(hashBytes);

            user.Password = hashString;
            user.RegisterTime = DateTime.Now;
            user.Salt = salt;
            user.UserRole = Role.CommonUser;
            user.IsActive = false;
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            SendmailAsync();
            async void SendmailAsync()
            {
                await Task.Factory.StartNew(() =>
                {
                    _redisDatabase.StringSet(user.UserName, salt);
                    _redisDatabase.KeyExpire(user.UserName, TimeSpan.FromMinutes(2));
                    SendEmail.Send(user.Email, "激活邮件", $"请点击下面的链接激活您的账户:<br/><a href='https://www.wulikunkun.com/LogIn/Verify?UserName={user.UserName}&ActiveCode={salt}'>https://www.wulikunkun.com/LogIn/Verify?UserName={user.UserName}&ActiveCode={salt}</a>");
                });
            }

            HttpContext.Session.SetString("username", user.Email);
            var jsonResult = Json(new { StateCode = 1 });
            return jsonResult;
        }

        public ViewResult Verify(string userName, string activeCode)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(activeCode)) Redirect(Url.Action("Index", "Home"));
            else
            {
                string currActiveCode = _redisDatabase.StringGet(userName).ToString();

                if (currActiveCode == activeCode)
                {
                    ViewBag.Info = "验证成功";
                    IdnentityUser user = dbContext.Users.FirstOrDefault(item => item.UserName == userName);
                    if (user != null)
                        user.IsActive = true;
                    else
                        ViewBag.Info = "验证链接参数有误，请重新获取！";
                }
                else if (currActiveCode == null || currActiveCode != activeCode)
                    ViewBag.Info = "验证链接已经失效，请重新获取！";
            }
            return View();
        }
    }
}