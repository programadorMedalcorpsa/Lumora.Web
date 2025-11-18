using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
