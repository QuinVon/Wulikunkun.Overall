using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Wulikunkun.Web.Models;
using Microsoft.AspNetCore.Http;

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
                string username = HttpContext.Session.GetString("username");
                if (username != null)
                {
                    ViewBag.UserName = username;
                }
            }
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

        public ViewResult Editor()
        {
            return View();
        }

        public void Edit(string editContent)
        {

        }

        #region 框架自带代码
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

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
