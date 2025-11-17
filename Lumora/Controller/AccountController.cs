using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controller
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
