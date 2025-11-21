using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Admin.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "¡La aplicación funciona correctamente!";
            ViewBag.Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return View();
        }
    }
}
