using Microsoft.AspNetCore.Mvc;

namespace Wulikunkun.Web.Controllers
{
    public class ContentController : Controller
    {
        public IActionResult Editor()
        {
            return View();
        }
    }
}