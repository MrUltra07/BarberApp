using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BarberApp.Controllers
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

		[HttpGet]
		public IActionResult ActiveAppointments()
		{
			var customerIdString = HttpContext.Session.GetString("CustomerId");

			if (string.IsNullOrEmpty(customerIdString))
			{
				return Unauthorized("Kullanıcı oturumu bulunamadı. Lütfen tekrar giriş yapın.");
			}

			if (!int.TryParse(customerIdString, out int customerId))
			{
				return Unauthorized("Geçersiz müşteri kimliği. Lütfen tekrar giriş yapın.");
			}

			var appointments = _context.Appointments
				.Include(a => a.Employee)   // Çalışan bilgisi
				.Include(a => a.Skill)      // Yetenek bilgisi
				.Include(a => a.Status)     // Durum bilgisi
				.Where(a => a.Customer.Id == customerId && a.Date >= DateTime.UtcNow) // Gelecekteki randevular
				.OrderBy(a => a.Date) // Tarihe göre sıralama
				.ToList();

			if (appointments == null || !appointments.Any())
			{
				ViewBag.Message = "Aktif randevularınız bulunmamaktadır.";
			}

			// Aktif randevuları View'a iletmek için Model olarak gönderiyoruz
			return View("~/Views/Appointments/ActiveAppointments/Index.cshtml", appointments);
		}

		public async Task<IActionResult> CancelAppointment(int id)
		{
			// Kullanıcının kimlik bilgisi oturumdan alınıyor
			var customerIdString = HttpContext.Session.GetString("CustomerId");

			if (string.IsNullOrEmpty(customerIdString))
			{
				return Unauthorized("Kullanıcı oturumu bulunamadı. Lütfen tekrar giriş yapın.");
			}

			if (!int.TryParse(customerIdString, out int customerId))
			{
				return Unauthorized("Geçersiz müşteri kimliği. Lütfen tekrar giriş yapın.");
			}

			// Randevu bulunuyor
			var appointment = await _context.Appointments
				.Include(a => a.Status)
				.FirstOrDefaultAsync(a => a.Id == id && a.Customer.Id == customerId);

			if (appointment == null)
			{
				TempData["ErrorMessage"] = "Belirtilen randevu bulunamadı veya bu randevuya erişim izniniz yok.";
				return RedirectToAction("ActiveAppointments", "Appointments");
			}

			if (appointment.Status != null && appointment.Status.Name == "Request")
			{
				var cancelledStatus = _context.AppointmentStatuses
					.FirstOrDefault(s => s.Name == "Cancelled");

				if (cancelledStatus != null)
				{
					appointment.Status = cancelledStatus;
					_context.Appointments.Update(appointment);
					await _context.SaveChangesAsync();

					TempData["SuccessMessage"] = "Randevunuz başarıyla iptal edilmiştir.";
				}
				else
				{
					TempData["ErrorMessage"] = "İptal durumu bulunamadı.";
				}
			}
			else
			{
				TempData["ErrorMessage"] = "Randevu durumu zaten iptal edilmiştir veya geçerli değil.";
			}

			// Aktif randevuları listele ve ActiveAppointments view'ına yönlendir
			return RedirectToAction("ActiveAppointments", "Appointments");
		}


	}
}
