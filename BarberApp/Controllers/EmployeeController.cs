using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using BarberApp.Helpers;

namespace BarberApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult AdminOnlyPage()
        {
            var employeeId = int.Parse(HttpContext.Session.GetString("EmployeeId"));

            if (!RoleHelper.IsAdmin(_context, employeeId))
            {
                return Forbid(); // Yetkisiz erişim
            }

            return View();
        }

        [HttpGet]
        public IActionResult LimitedAccessPage()
        {
            var employeeId = int.Parse(HttpContext.Session.GetString("EmployeeId"));

            // Eğer çalışanın bir ADMIN skill'i varsa tam erişim sağla
            if (RoleHelper.IsAdmin(_context, employeeId))
            {
                return View("AdminView");
            }

            // Sadece sınırlı işlemlere izin ver
            return View("LimitedView");
        }
    }
}
