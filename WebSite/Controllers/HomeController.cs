using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using wulikunkun_dotnet_core_mvc.Models;
using Newtonsoft.Json;

namespace wulikunkun_dotnet_core_mvc.Controllers
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

        #region 返回视图的Action

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult AdminIndex()
        {
            return View();
        }

        public ViewResult PageDetail()
        {
            return View();
        }

        #endregion

        public JsonResult CreateUser(User user)
        {
            object result = null;
            if (dbContext.Users.Any(item => item.UserName == user.UserName))
            {
                result = new
                {
                    Message = "已经存在相同的用户名！",
                    StateCode = 0.1
                };
                return Json(result);
            }

            if (dbContext.Users.Any(item => item.Email == user.Email))
            {
                result = new
                {
                    Message = "该邮箱已经注册！",
                    StateCode = 0.2
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
            user.UserRole = Role.User;
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            result = new
            {
                Message = "注册成功！",
                StateCode = 1
            };
            JsonResult jsonResult = Json(result);
            return jsonResult;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Swagger测试部分

        [HttpGet("{id}")]
        public ActionResult<string> GetUsers(int id)
        {
            return $"Get Num {id} Info";
        }

        [HttpPost("{id}")]
        public ActionResult<string> Edit(int id)
        {
            return $"Modify Num {id} info";
        }

        [HttpPut("{id}")]
        public ActionResult<string> Add(int id)
        {
            return $"Add Num {id} info";
        }

        ///summary
        ///to get user info
        ///summary
        [HttpPost]
        [Route("UserInfo")]
        public JsonResult UserInfo(string uid, string password, string time)
        {
            object responseData = new
            {
                responseDescription = "响应成功",
                responseState = "0",
                UserInfo = new
                {
                    userId = "stu001",
                    firstName = "刘晓阳",
                    lastName = "数学学院"
                }
            };
            return Json(responseData);
        }

        [HttpDelete("{id}")]
        public ActionResult<string> Delete(int id)
        {
            return $"Delete Num {id} info";
        }
        #endregion
    }
}
