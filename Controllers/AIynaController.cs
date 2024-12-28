using Microsoft.AspNetCore.Mvc;

namespace BarberApp.Controllers
{
    public class AIynaController : Controller
    {
        // Sadece view döndüren basit bir yöntem
        public IActionResult Index()
        {
            return View("~/Views/AIyna/Index.cshtml");
        }
    }
}
