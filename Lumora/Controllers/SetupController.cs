using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers
{
    public class SetupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
