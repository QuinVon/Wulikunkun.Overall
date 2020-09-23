using Microsoft.AspNetCore.Mvc;

namespace Wulikunkun.Web.Controllers
{
    public class ArticleController : Controller
    {
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