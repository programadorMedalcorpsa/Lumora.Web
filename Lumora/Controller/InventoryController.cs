using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controller
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
