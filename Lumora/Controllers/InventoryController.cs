using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
