using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using BarberApp.ViewModels;
using BarberApp.Helpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        [HttpGet("skills")]
        public IActionResult Skills()
        {
            var skills = _context.Skills.ToList(); // Tüm skill'leri getir
            return View("Skills/Index",skills); // Skills.cshtml'e skill'leri gönder
        }

        [HttpGet("skills/create")]
        public IActionResult CreateSkill()
        {
            return View("Skills/Create/Index"); // CreateSkill.cshtml dosyasını döner
        }
        [HttpPost("skills/create")]
        public IActionResult CreateSkill(Skill newSkill)
        {
            if (ModelState.IsValid)
            {
                _context.Skills.Add(newSkill);
                _context.SaveChanges();
                return RedirectToAction("skills"); // Skill listeleme sayfasına yönlendir
            }

            return View("Skills/Create/Index",newSkill); // Model hatalıysa formu tekrar göster
        }

        [HttpGet("skills/edit/{id}")]
        public IActionResult EditSkill(int id)
        {
            var skill = _context.Skills.FirstOrDefault(s => s.Id == id);
            if (skill == null)
            {
                return NotFound(); // Eğer ID bulunamazsa 404 döndür
            }
            return View("Skills/Edit/Index",skill); // EditSkill.cshtml'e skill'i gönder
        }


        [HttpPost("Skills/Edit/{id}")]
        public IActionResult EditSkill(int id, Skill updatedSkill)
        {
            var skill = _context.Skills.FirstOrDefault(s => s.Id == id);
            if (skill == null)
            {
                return NotFound(); // Eğer ID bulunamazsa 404 döndür
            }

            // Skill'i güncelle
            skill.Title = updatedSkill.Title;
            skill.Description = updatedSkill.Description;
            skill.Price = updatedSkill.Price;
            skill.Bonus = updatedSkill.Bonus;
            skill.Duration = updatedSkill.Duration;
            skill.Cost = updatedSkill.Cost;
            skill.IsVisible = updatedSkill.IsVisible;

            _context.SaveChanges(); // Değişiklikleri kaydet

            return RedirectToAction("skills"); // Listeleme sayfasına yönlendir
        }
        [HttpGet("employees")]
        public IActionResult Employees()
        {
            var employees = _context.Employees.Include(e=>e.Skills).ToList(); // Tüm çalışanları getir
            return View("Employee/Index",employees); // Employees/Index.cshtml'e gönder
        }
        [HttpGet("employees/create")]
        public IActionResult CreateEmployee()
        {
            var viewModel = new EmployeeViewModel
            {
                Employee = new Employee
                {
                    Name = string.Empty,
                    Surname = string.Empty,
                    ProfileImageUrl = string.Empty,
                    IdNumber = string.Empty,
                    Iban = string.Empty,
                    BasicWage = 0,
                    Password = string.Empty
                },
                Skills = _context.Skills.ToList(),
                SelectedSkillIds = new List<int>()
            };

            return View("Employee/Create/Index", viewModel); // Create Employee view'i göster
        }

        [HttpPost("employees/create")]
        public IActionResult CreateEmployee(EmployeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var newEmployee = viewModel.Employee;

                // Seçilen yetenekleri ekle
                foreach (var skillId in viewModel.SelectedSkillIds)
                {
                    var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
                    if (skill != null)
                    {
                        newEmployee.Skills.Add(skill);
                    }
                }

                _context.Employees.Add(newEmployee);
                _context.SaveChanges();

                return RedirectToAction("Employees", new { status = "ok", message = "Employee created successfully!" });
            }

            // Eğer ModelState geçerli değilse Skills'i yeniden doldurun
            viewModel.Skills = _context.Skills.ToList();
            return View("Employee/Create/Index", viewModel);
        }



        [HttpGet("employees/edit/{id}")]
        public IActionResult EditEmployee(int id)
        {
            var employee = _context.Employees
                .Include(e => e.Skills)
                .FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var viewModel = new EmployeeViewModel
            {
                Employee = employee,
                Skills = _context.Skills.ToList(),
                SelectedSkillIds = employee.Skills.Select(s => s.Id).ToList()
            };

            return View("Employee/Edit/Index", viewModel);
        }

        [HttpPost("employees/edit/{id}")]
        public IActionResult EditEmployee(int id, EmployeeViewModel viewModel)
        {
            if (viewModel.SelectedSkillIds == null || !viewModel.SelectedSkillIds.Any())
            {
                TempData["Status"] = "error";
                TempData["Message"] = "Please select at least one skill.";
                return RedirectToAction("Employees");
            }

            var employee = _context.Employees
                .Include(e => e.Skills)
                .FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                TempData["Status"] = "error";
                TempData["Message"] = "Employee not found.";
                return RedirectToAction("Employees");
            }

            // Çalışan bilgilerini güncelleme
            employee.Name = viewModel.Employee.Name;
            employee.Surname = viewModel.Employee.Surname;
            employee.ProfileImageUrl = viewModel.Employee.ProfileImageUrl;
            employee.IdNumber = viewModel.Employee.IdNumber;
            employee.Iban = viewModel.Employee.Iban;
            employee.BasicWage = viewModel.Employee.BasicWage;
            employee.Password = viewModel.Employee.Password;

            // Skill'leri güncelleme
            employee.Skills.Clear();
            foreach (var skillId in viewModel.SelectedSkillIds)
            {
                var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
                if (skill != null)
                {
                    employee.Skills.Add(skill);
                }
            }

            _context.SaveChanges();
            TempData["Status"] = "ok";
            TempData["Message"] = "Employee updated successfully!";
            return RedirectToAction("Employees");
        }





        [HttpGet("employees/delete/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return RedirectToAction("Employees", new { status = "ok", message = "Employee deleted successfully!" });
        }



    }
}
