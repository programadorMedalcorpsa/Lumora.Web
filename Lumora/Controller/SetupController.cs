using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controller
{
    public class SetupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
