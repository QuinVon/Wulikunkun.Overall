using Microsoft.AspNetCore.Mvc;

namespace Wulikunkun.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}