using Microsoft.AspNetCore.Mvc;

namespace Lumora.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login() => View();     // ← Funciona
        public IActionResult Dashboard() => View(); // ← Funciona
        public RedirectToActionResult Logout() => RedirectToAction("Login");
    }
}
