using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BarberApp.Controllers
{
    [Route("admin/appointments")]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private static readonly string viewAdress = "~/Views/Admin/Appointment/";

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        // Admin veya çalışan girişine göre randevuları listeleyen metod
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Session'dan EmployeeId'yi al
            var employeeIdString = HttpContext.Session.GetString("EmployeeId");

            if (employeeIdString == null)
            {
                // Eğer session'da EmployeeId yoksa, kullanıcı login sayfasına yönlendirilir
                return RedirectToAction("Index", "Login");
            }

            int employeeId = int.Parse(employeeIdString);

            // Employee bilgilerini session'dan alınan EmployeeId ile sorgula
            var employee = await _context.Employees
                                         .Include(e => e.Skills)
                                         .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return NotFound(); // Çalışan bulunamadıysa
            }

            var isAdmin = employee.Skills.Any(s => s.Title == "ADMIN");

            // Admin kullanıcıları tüm randevuları görebilir, çalışanlar yalnızca kendi randevularını
            IQueryable<Appointment> appointmentsQuery;

            if (isAdmin)
            {
                appointmentsQuery = _context.Appointments
                                             .Include(a => a.Employee)
                                             .Include(a => a.Customer)
                                             .Include(a => a.Skill)
                                             .Include(a => a.Status)
                                             .Where(a => a.Status.Name != "Completed") // "Completed" olmayan randevuları al
                                             .OrderByDescending(a => a.Date); // Tarihe göre sıralama (yeni tarih en üstte)
            }
            else
            {
                appointmentsQuery = _context.Appointments
                                             .Where(a => a.Employee.Id == employee.Id && a.Status.Name != "Completed") // "Completed" olmayan randevuları al
                                             .Include(a => a.Employee)
                                             .Include(a => a.Customer)
                                             .Include(a => a.Skill)
                                             .Include(a => a.Status)
                                             .OrderByDescending(a => a.Date); // Tarihe göre sıralama (yeni tarih en üstte)
            }

            // Randevuları al
            var appointments = await appointmentsQuery.ToListAsync();

            if (!appointments.Any())
            {
                ViewBag.ErrorMessage = "Hiç randevu bulunamadı.";
            }

            ViewBag.Statuses = await _context.AppointmentStatuses.ToListAsync();

            return View(viewAdress + "Index.cshtml", appointments); // Direkt View'e gönder
        }


        // Status güncelleme işlemi
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int appointmentId, int statusId)
        {
            var appointment = await _context.Appointments
                                             .Include(a => a.Status)
                                             .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            // Eğer randevu tamamlanmışsa, başka bir sayfaya yönlendir
            if (appointment.Status.Name == "Completed")
            {
                // Randevu tamamlanmışsa, başka bir sayfaya yönlendir
                return RedirectToAction("CompletedAppointments", "Appointment");
            }

            // Durumu güncelle
            var status = await _context.AppointmentStatuses.FindAsync(statusId);
            if (status != null)
            {
                if(status.Name == "Completed")
                {
                    return RedirectToAction("CompletedAppointments", "Appointment");

                }
                appointment.Status = status;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // Sayfayı yenile
        }

        // Completed statüsündeki randevular için bir yönlendirme sayfası
        [HttpGet("completed-appointments")]
        public IActionResult CompletedAppointments()
        {
            return View(); // Tamamlanmış randevular için yönlendirme yapılacak başka bir sayfa (view)
        }
    }
}

