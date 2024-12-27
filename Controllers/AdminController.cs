using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using System.Data.Entity;
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
                var IsAdminString = HttpContext.Session.GetString("IsAdmin");

                return RedirectToAction("Dashboard", "Admin", new { isAdmin = IsAdminString });
            }

            // View'in tam yolunu belirtin
            return View("Login/Index");
        }

        [HttpPost("login")]
        public IActionResult Login(string idNumber, string password)
        {
            try
            {
                var employee = _context.Employees.Select(
                    e => new
                    {
                        e.Id,
                        e.IdNumber,
                        e.Password,
                        e.Name,
                        e.Surname,
                        Skills = e.Skills.Select(skill => skill.Title).ToList()
                    })
                    .FirstOrDefault(e => e.IdNumber == idNumber && e.Password == password); // Şifrelenmiş şifre kontrolü önerilir.

                if (employee == null)
                {
                    ViewBag.ErrorMessage = "Giriş başarısız: Lütfen kimlik bilgilerinizi kontrol edin.";
                    return View("Login/Index");
                }

                // Oturum bilgilerini ayarla
                HttpContext.Session.SetString("EmployeeId", employee.Id.ToString());
                var adminDetails = _context.Employees
                .Select(e => new
                {
                    e.Name,
                    e.Surname,
                    Skills = e.Skills.Select(skill => skill.Title).ToList()
                })
                .ToList();
                if (employee.Skills.Any(s => s == "ADMIN"))
                {
                    HttpContext.Session.SetString("IsAdmin", "true");
                }
                else
                {
                    if(employee == null)
                    {
                        HttpContext.Session.SetString("IsAdmin", "falsenull");

                    }
                    else
                    {

                        var skillTitles = "ses";
                        foreach (var skill in employee.Skills)
                        {
                            skillTitles += skill + "-";
                        }

                        HttpContext.Session.SetString("IsAdmin", "false" + skillTitles + " " + employee.Name);

                    }
                    HttpContext.Session.SetString("IsEmployee", "true");

                }
                var IsAdminString = HttpContext.Session.GetString("IsAdmin");
                return RedirectToAction("Dashboard", "Admin", new {isAdmin=IsAdminString});
            }
            catch (Exception ex)
            {
                // Hata loglaması
                Console.WriteLine($"Hata: {ex.Message}"); // Burada bir loglama kütüphanesi kullanmanız önerilir.
                ViewBag.ErrorMessage = "Bir hata oluştu. Lütfen tekrar deneyin."; // Kullanıcıya bir hata mesajı döndür.
                return View("Login/Index"); // Hata durumunda tekrar giriş sayfasına döndür.
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
