using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using BarberApp.Helpers;

namespace BarberApp.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("EmployeeId") != null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            // View'in tam yolunu belirtin
            return View("Login/Index");
        }

        [HttpPost("login")]
        public IActionResult Login(string idNumber, string password)
        {
            try
            {
                var employee = _context.Employees
                    .FirstOrDefault(e => e.IdNumber == idNumber && e.Password == password);

                if (employee == null)
                {
                    ViewBag.ErrorMessage = "Geçersiz kimlik bilgileri.";
                    return View("Login");
                }
                Console.WriteLine("deneme");
                Console.WriteLine(password, idNumber);
                Console.WriteLine("deneme");

                HttpContext.Session.SetString("EmployeeId", employee.Id.ToString());
                return RedirectToAction("Dashboard", "Admin");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                throw;
            }
        }


        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Admin");
        }
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            // Oturumdaki Employee ID'yi al
            var employeeId = int.Parse(HttpContext.Session.GetString("EmployeeId") ?? "0");

            // Kullanıcının Admin olup olmadığını kontrol edin
            if (!_context.Employees
                         .Any(e => e.Id == employeeId && e.Skills.Any(s => s.Title == "ADMIN")))
                         //.Any(e => e.Id == employeeId))
            {
                return Forbid(); // Yetkisiz erişim
            }

            // Admin sayfasında görüntülenecek veriler
            var adminDetails = _context.Employees
                .Select(e => new
                {
                    e.Name,
                    e.Surname,
                    Skills = e.Skills.Select(skill => skill.Title).ToList()
                })
                .ToList();

            ViewBag.AdminDetails = adminDetails;

            return View("Dashboard/Index");
        }


    }
}
