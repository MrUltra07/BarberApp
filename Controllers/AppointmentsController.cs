using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BarberApp.Controllers
{
	public class AppointmentsController : BaseController
	{
		private readonly AppDbContext _context;

		public AppointmentsController(AppDbContext context) : base(context)
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
			var customerIdString = HttpContext.Session.GetString("CustomerId");

			if (string.IsNullOrEmpty(customerIdString) || !int.TryParse(customerIdString, out int customerId))
			{
				return RedirectToAction("Login", "Customer");
			}

			var completedAppointments = _context.Appointments
				.Where(a => a.Customer.Id == customerId && (a.Status.Id== 3 || a.Status.Id == 3)) // StatusId'si 4 olan randevular
				.Include(a => a.Employee)    // Çalışan bilgisi
				.Include(a => a.Skill)       // Yetenek bilgisi
				.Include(a => a.Status)      // Durum bilgisi
				.OrderByDescending(a => a.Date) // Tarihe göre sıralama (en yeni üstte)
				.ToList();

			if (completedAppointments == null || !completedAppointments.Any())
			{
				ViewBag.Message = "Geçmiş randevunuz bulunmamaktadır.";
			}

			return View("PastAppointments/Index", completedAppointments);
		}




		[HttpGet]
		public IActionResult ActiveAppointments()
		{
			var customerIdString = HttpContext.Session.GetString("CustomerId");

			if (string.IsNullOrEmpty(customerIdString) || !int.TryParse(customerIdString, out int customerId))
			{
				return Unauthorized("Kullanıcı oturumu bulunamadı. Lütfen tekrar giriş yapın.");
			}

			var appointments = _context.Appointments
				.Include(a => a.Employee)   // Çalışan bilgisi
				.Include(a => a.Skill)      // Yetenek bilgisi
				.Include(a => a.Status)     // Durum bilgisi
				.Where(a => a.Customer.Id == customerId
							&& a.Status.Name == "Request" // Sadece "Request" durumundaki randevular
							&& a.Date >= DateTime.UtcNow) // Gelecekteki randevular
				.OrderBy(a => a.Date) // Tarihe göre sıralama
				.ToList();

			if (!appointments.Any())
			{
				ViewBag.Message = "Aktif randevularınız bulunmamaktadır.";
			}

			return View("~/Views/Appointments/ActiveAppointments/Index.cshtml", appointments);
		}


		[HttpPost]
		public async Task<IActionResult> CancelAppointment(int id)
		{
			var customerIdString = HttpContext.Session.GetString("CustomerId");

			if (string.IsNullOrEmpty(customerIdString) || !int.TryParse(customerIdString, out int customerId))
			{
				return BadRequest("Oturum bulunamadı veya geçersiz müşteri kimliği.");
			}

			var appointment = await _context.Appointments
				.Include(a => a.Status)
				.FirstOrDefaultAsync(a => a.Id == id && a.Customer.Id == customerId);

			if (appointment == null)
			{
				return NotFound("Randevu bulunamadı veya erişim yetkiniz yok.");
			}

			if (appointment.Status.Name == "Request")
			{
				var cancelledStatus = await _context.AppointmentStatuses
					.FirstOrDefaultAsync(s => s.Name == "Cancelled");

				if (cancelledStatus != null)
				{
					appointment.Status = cancelledStatus;
					_context.Appointments.Update(appointment);
					await _context.SaveChangesAsync();
					return Ok();
				}
			}

			return BadRequest("Randevu iptal edilemedi.");
		}




	}
}
