using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wulikunkun.Utility;
using Wulikunkun.Web.Models;

namespace Wulikunkun.Web.Controllers
{
    public class RegisterUser : ApplicationUser
    {
        public string Password { get; set; }
    }
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        private static readonly ConnectionMultiplexer _multiplexer = ConnectionMultiplexer.Connect($"localhost:6379,password={ Environment.GetEnvironmentVariable("RedisPassword")}");
        private static readonly IDatabase _redisDatabase = _multiplexer.GetDatabase();



        public RegisterController(ILogger<RegisterController> logger, ApplicationDbContext ApplicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            dbContext = ApplicationDbContext;
            _logger = logger;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateUserAsync(RegisterUser user)
        {
            var taskOne = _userManager.FindByEmailAsync(user.Email);
            if (taskOne.Result != null)
            {
                return Json(new { StateCode = 2 });
            }
            var taskTwo = _userManager.FindByNameAsync(user.UserName);
            if (taskTwo.Result != null)
            {
                return Json(new { StateCode = 3 });
            }

            /* 创建用户 */
            var newUser = new ApplicationUser()
            {
                Email = user.Email,
                UserName = user.UserName,
                RegisterTime = DateTime.Now,
                IsActive = false
            };
            IdentityResult createUserResult = await _userManager.CreateAsync(newUser, user.Password);

            /* 创建用户角色 */
            bool isExisit = await _roleManager.RoleExistsAsync("CommonUser");
            if (!isExisit)
            {
                var role = new IdentityRole();
                role.Name = "CommonUser";
                await _roleManager.CreateAsync(role);
                if (createUserResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "CommonUser");
                }
            }


            /* 临时取消发送邮件验证 */
            /* SendmailAsync();
            async void SendmailAsync()
            {
                await Task.Factory.StartNew(() =>
                {
                    string verifyNum = Guid.NewGuid().ToString();

                    _redisDatabase.StringSet(user.UserName, verifyNum);
                    _redisDatabase.KeyExpire(user.UserName, TimeSpan.FromMinutes(2));
                    SendEmail.Send(user.Email, "激活邮件", $"请点击下面的链接激活您的账户:<br/><a href='https://www.wulikunkun.com/Register/Verify?UserName={user.UserName}&ActiveCode={verifyNum}'>https://www.wulikunkun.com/Register/Verify?UserName={user.UserName}&ActiveCode={verifyNum}</a>");
                });
            } */

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

                    ApplicationUser user = _userManager.FindByNameAsync(userName).Result;
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