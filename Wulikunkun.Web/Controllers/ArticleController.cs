using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Wulikunkun.Web.Controllers
{
    public class ArticleController : Controller
    {
        [Authorize]
        public IActionResult Editor()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}