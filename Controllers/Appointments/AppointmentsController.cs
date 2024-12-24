using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BarberApp.Controllers.Appointments
{
	public class AppointmentsController : Controller
	{
		private readonly AppDbContext _context;

		public AppointmentsController(AppDbContext context)
		{
			_context = context;
		}

		// Randevu almak için yönlendiren aksiyon
		[HttpGet]
		public IActionResult BookAppointment()
		{
			var customerName = HttpContext.Session.GetString("CustomerName");

			if (string.IsNullOrEmpty(customerName))
			{
				// Kullanıcı giriş yapmamışsa, giriş yapma mesajı gösterilecek
				ViewBag.ErrorMessage = "Lütfen giriş yapın.";
				return View("Index");
			}

			// Kullanıcı giriş yaptıysa randevu al sayfasına yönlendirme yapılır
			ViewBag.Employees = _context.Employees.ToList();
			ViewBag.Skills = _context.Skills.ToList();
			ViewBag.Statuses = _context.AppointmentStatuses.ToList();

			return View("BookAppointment/Index");
		}

		[HttpPost]
		[HttpPost]
		public IActionResult BookAppointment(Appointment appointment)
		{
			if (ModelState.IsValid)
			{
				// Kullanıcı giriş yaptıysa
				var customerName = HttpContext.Session.GetString("CustomerName");

				// Eğer tarih geçmişse, hata mesajı göster
				if (appointment.Date < DateTime.Now)
				{
					ViewBag.ErrorMessage = "Randevu tarihi geçmiş bir zaman olamaz!";
					return View();
				}

				// Randevu saatinin uygun olup olmadığını kontrol et
				var availableTimes = _context.AvailableTimes
					.Where(a => a.DayIndex == (int)appointment.Date.DayOfWeek)
					.ToList();

				var appointmentTime = appointment.Date.TimeOfDay;

				bool isAvailable = false;
				foreach (var availableTime in availableTimes)
				{
					if (appointmentTime >= availableTime.StartTime && appointmentTime <= availableTime.EndTime)
					{
						isAvailable = true;
						break;
					}
				}

				if (!isAvailable)
				{
					ViewBag.ErrorMessage = "Seçtiğiniz saat, geçerli çalışma saatleri dışında!";
					return View();
				}

				// Eğer tarih uygun ve zaman dilimi geçerliyse, randevuyu kaydet
				_context.Appointments.Add(appointment);
				_context.SaveChanges();

				return RedirectToAction("PastAppointments", "Appointments");
			}

			return View();
		}


		// Geçmiş randevuları görüntülemek için aksiyon
		[HttpGet]
		public IActionResult PastAppointments()
		{
			var customerId = HttpContext.Session.GetString("CustomerId");
			if (string.IsNullOrEmpty(customerId))
			{
				return RedirectToAction("Login", "Customer");
			}

			var pastAppointments = _context.Appointments
				.Where(a => a.Customer.Id == int.Parse(customerId))
				.Include(a => a.Employee)
				.Include(a => a.Skill)
				.Include(a => a.Status)
				.ToList();

			return View("PastAppointments/Index");
		}
	}
}
