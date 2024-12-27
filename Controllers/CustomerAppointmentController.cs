using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using BarberApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Globalization;

namespace BarberApp.Controllers
{
    public class CustomerAppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomerAppointmentController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: BookAppointment page
        [HttpGet]
        public IActionResult BookAppointment()
        {
            var customerName = HttpContext.Session.GetString("CustomerName");

            if (string.IsNullOrEmpty(customerName))
            {
                ViewBag.ErrorMessage = "Lütfen giriş yapın.";
                return RedirectToAction("Login", "Customer");
            }

            // Tüm skill'leri gönderiyoruz
            ViewBag.Skills = _context.Skills.ToList().Where((s) => s.IsVisible==true);
            return View("~/Views/Appointments/BookAppointment/Index.cshtml", new BookAppointmentViewModel());
        }

        // POST: BookAppointment
        [HttpPost]
        public async Task<IActionResult> BookAppointment(BookAppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Skills = _context.Skills.ToList().Where((s) => s.IsVisible==true);
                return View(model);
            }

            // Kullanıcının oturum bilgisi
            // Kullanıcı oturumunda "CustomerId" mevcut mu kontrol edin
            var customerIdString = HttpContext.Session.GetString("CustomerId");

            if (string.IsNullOrEmpty(customerIdString))
            {
                return Unauthorized("Kullanıcı oturumu bulunamadı. Lütfen tekrar giriş yapın.");
            }

            // "CustomerId" string'ini int'e dönüştürün
            if (!int.TryParse(customerIdString, out int customerId))
            {
                return Unauthorized("Geçersiz müşteri kimliği. Lütfen tekrar giriş yapın.");
            }
            // Skill ve employee doğrulaması
            var employee = _context.Employees.Include(e => e.Skills).FirstOrDefault(e => e.Id == model.EmployeeId);
            var skill = _context.Skills.FirstOrDefault(s => s.Id == model.SkillId);

            if (employee == null || skill == null || !employee.Skills.Any(s => s.Id == model.SkillId))
            {
                ViewBag.ErrorMessage = "Seçilen skill ve employee eşleşmiyor.";
                ViewBag.Skills = _context.Skills.ToList().Where((s) => s.IsVisible==true);
                return View("~/Views/Appointments/BookAppointment/Index.cshtml", model);
            }
            

            // Uygun saat kontrolü (API çağrısı)
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(
                $"https://localhost:7132/api/schedules/available-slots?skillId={model.SkillId}&employeeId={model.EmployeeId}&date={model.AppointmentDate:yyyy-MM-dd}"
            );

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Uygun saatler alınamadı. Lütfen tekrar deneyin.";
                ViewBag.Skills = _context.Skills.ToList().Where((s) => s.IsVisible==true);
                return View("~/Views/Appointments/BookAppointment/Index.cshtml", model);
            }

            var availableSlots = JsonSerializer.Deserialize<List<string>>(await response.Content.ReadAsStringAsync());

            if (availableSlots == null || !availableSlots.Contains(model.TimeSlot))
            {
                ViewBag.ErrorMessage = "Seçilen saat uygun değil. Lütfen başka bir saat seçin.";
                ViewBag.Skills = _context.Skills.ToList().Where((s) => s.IsVisible==true);
                return View("~/Views/Appointments/BookAppointment/Index.cshtml", model);
            }

            // Randevu oluştur
            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
            var status = _context.AppointmentStatuses.FirstOrDefault(s => s.Name == "Request");
            DateTime appointmentDateTime;
            string combinedDateTime = $"{model.AppointmentDate:yyyy.MM.dd} {model.TimeSlot}";

            if (!DateTime.TryParseExact(combinedDateTime,
                                       new[] { "yyyy.MM.dd HH:mm:ss", "yyyy-MM-dd HH" },
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out appointmentDateTime))
            {
                ViewBag.ErrorMessage = $"Geçerli bir tarih ve saat seçin. Girilen değer: {combinedDateTime}";
                ViewBag.Skills = _context.Skills.ToList().Where((s) => s.IsVisible == true);
                return View("~/Views/Appointments/BookAppointment/Index.cshtml", model);
            }
            // Tarih kontrolü
            
            if (appointmentDateTime < DateTime.Now)
            {
                ViewBag.ErrorMessage = "Geçmiş bir tarih seçilemez!" + model.AppointmentDate.ToString();
                ViewBag.Skills = _context.Skills.ToList().Where((s) => s.IsVisible == true);
                return View("~/Views/Appointments/BookAppointment/Index.cshtml", model);
            }
            // Tarihi UTC'ye dönüştür
            TimeZoneInfo utcPlus3 = TimeZoneInfo.CreateCustomTimeZone("UTC+3", TimeSpan.FromHours(3), "UTC+3", "UTC+3");
            appointmentDateTime = TimeZoneInfo.ConvertTimeToUtc(appointmentDateTime, utcPlus3);

            var appointment = new Appointment
            {
                Date = appointmentDateTime,
                Customer = customer,
                Employee = employee,
                Skill = skill,
                Status = status
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            ViewBag.SuccessMessage = "Randevu başarıyla oluşturuldu!";
            return RedirectToAction("ActiveAppointments", "Appointments");
        }

		

	}
}
