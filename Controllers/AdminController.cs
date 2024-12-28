using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using BarberApp.ViewModels;
using System.Data.Entity;
using System.Security.Claims;
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
            // Kullanıcının rolünü kontrol edin
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            // Geçerli tarih bilgisi
            var currentDate = DateTime.UtcNow;

            // Randevu ve kazanç verilerini çekme
            var appointmentsQuery = _context.Invoices
                .Where(i => i.Appointment.Date >= currentDate.AddMonths(-12) && i.Appointment.Date <= currentDate);

            // Eğer admin değilse sadece o anki employeeId'ye sahip olan randevuları al
            if (!isAdmin)
            {
                var employeeId = Int32.Parse(HttpContext.Session.GetString("EmployeeId"));
                appointmentsQuery = appointmentsQuery.Where(i => i.Appointment.Employee.Id == employeeId);
            }

            var appointments = appointmentsQuery
            .GroupBy(i => new { Year = i.Appointment.Date.Year, Month = i.Appointment.Date.Month })
            .Select(g => new AppointmentViewModel
            {
                MonthYear = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                TotalIncome = isAdmin
                    ? g.Sum(i => i.Appointment.Skill.Price) - g.Sum(i => i.Appointment.Skill.Bonus) - g.Sum(i => i.Appointment.Skill.Cost)
                    : g.Sum(i => i.TipAmount) + g.Sum(i => i.Appointment.Skill.Bonus), // Çalışan kazancı
                EmployeeId = g.FirstOrDefault().Appointment.Employee.Id,
                IsAdmin = isAdmin, // IsAdmin bilgisini ekle
                KazancData = g.Select(i => new KazancDataViewModel
                {
                    SkillName = i.Appointment.Skill.Title,
                    Kazanc = isAdmin
                        ? (i.Appointment.Skill.Price - i.Appointment.Skill.Bonus - i.Appointment.Skill.Cost)
                        : (i.TipAmount + i.Appointment.Skill.Bonus), // Her randevunun kazancını hesapla
                    Date = i.Appointment.Date.ToString("yyyy-MM-dd"), // Tarih formatı
                    EmployeeName = i.Appointment.Employee.Name // Çalışan adını ekle
                }).ToList()
            }).ToList();

            // Eğer admin değilse Basic Wage'yi ekle
            if (!isAdmin)
            {
                var employeeId = Int32.Parse(HttpContext.Session.GetString("EmployeeId"));
                var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId); // Employee'yi veritabanından çekiyoruz

                if (employee != null)
                {
                    foreach (var appointment in appointments)
                    {
                        appointment.KazancData.Add(new KazancDataViewModel
                        {
                            SkillName = "Basic Wage",
                            Kazanc = employee.BasicWage // Basic Wage'yi ekle
                        });
                    }
                }
            }

            return View("Dashboard/Index", appointments);
        }

        [HttpGet("chart-data")]
        public IActionResult GetChartData()
        {
            // Kullanıcının rolünü kontrol edin
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            var employeeId = Int32.Parse(HttpContext.Session.GetString("EmployeeId"));

            // Geçerli tarih bilgisi
            var currentDate = DateTime.UtcNow;

            // Randevu ve kazanç verilerini çekme
            var appointmentsQuery = _context.Invoices
                .Where(i => i.Appointment.Date >= currentDate.AddMonths(-12) && i.Appointment.Date <= currentDate);

            // Eğer admin değilse sadece o anki employeeId'ye sahip olan randevuları al
            if (!isAdmin)
            {
                appointmentsQuery = appointmentsQuery.Where(i => i.Appointment.Employee.Id == employeeId);
            }

            var chartData = appointmentsQuery
                .GroupBy(i => new { Year = i.Appointment.Date.Year, Month = i.Appointment.Date.Month })
                .Select(g => new
                {
                    MonthYear = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    TotalIncome = isAdmin
                        ? g.Sum(i => i.Appointment.Skill.Price) - g.Sum(i => i.Appointment.Skill.Bonus) - g.Sum(i => i.Appointment.Skill.Cost)
                        : g.Sum(i => i.TipAmount) + g.Sum(i => i.Appointment.Skill.Bonus),
                    KazancData = g.Select(i => new
                    {
                        SkillName = i.Appointment.Skill.Title,
                        Kazanc = isAdmin
                            ? (i.Appointment.Skill.Price - i.Appointment.Skill.Bonus - i.Appointment.Skill.Cost)
                            : (i.TipAmount + i.Appointment.Skill.Bonus),
                        Date = i.Appointment.Date.ToString("yyyy-MM-dd"),
                        EmployeeName = i.Appointment.Employee.Name
                    }).ToList()
                })
                .ToList();

            // Eğer admin değilse Basic Wage'yi ekle
            if (!isAdmin)
            {
                var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId); // Employee'yi veritabanından çekiyoruz

                if (employee != null)
                {
                    foreach (var chartItem in chartData)
                    {
                        chartItem.KazancData.Add(new
                        {
                            SkillName = "Basic Wage",
                            Kazanc = employee.BasicWage, // Basic Wage'yi ekle
                            Date = DateTime.UtcNow.ToString("yyyy-MM-dd"), // Tarih bilgisi ekle
                            EmployeeName = employee.Name // Çalışan adını ekle
                        });
                    }
                }
            }

            return Json(chartData);
        }











    }
}
