using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using System.Linq;


namespace BarberApp.Controllers.Customer
{
	public class CustomerController : Controller
	{
		private readonly AppDbContext _context;

		public CustomerController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View("Login/Login"); // View: Views/Customer/Login/Login.cshtml
		}

		[HttpPost]
		public IActionResult Login(string email, string password)
		{
			try
			{
				// Veritabanında email ve şifreyi kontrol et
				var customer = _context.Customers
					.FirstOrDefault(c => c.Email == email && c.Password == password);

				if (customer == null)
				{
					// Eğer müşteri bulunmazsa hata mesajı göster
					ViewBag.ErrorMessage = "Geçersiz e-posta veya şifre.";
					return View(); // Hata mesajını göster ve tekrar giriş sayfasını render et
				}

				// Başarılı giriş
				Console.WriteLine("deneme");
				Console.WriteLine($"Email: {email}, Password: {password}");
				Console.WriteLine("deneme");

				// Session'a müşteri ID'sini kaydet
				HttpContext.Session.SetString("CustomerName", customer.Name);
				HttpContext.Session.SetString("CustomerId", customer.Id.ToString());

				// Ana sayfaya yönlendir
				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				// Hata oluşursa logla
				Console.WriteLine($"Hata: {ex.Message}");
				throw;
			}
		}

		public IActionResult Logout()
		{
			// Çıkış işlemi: Tüm session verilerini temizler
			HttpContext.Session.Clear();

			// Login sayfasına yönlendir
			return RedirectToAction("Login", "Customer");
		}


		[HttpGet]
		public IActionResult Register()
		{
			return View("Register/Register"); // View: Views/Customer/Register/Register.cshtml
		}

		[HttpPost]
		public IActionResult Register(BarberApp.Models.Customer model)
		{
			if (!ModelState.IsValid)
			{
				// Model valid değilse, hataları göster
				return View(model);
			}

			try
			{
				// Müşteri email kontrolü
				var existingCustomer = _context.Customers.FirstOrDefault(c => c.Email == model.Email);
				if (existingCustomer != null)
				{
					ViewBag.ErrorMessage = "Bu e-posta adresi zaten kayıtlı.";
					return View(model);
				}

				// Yeni müşteri kaydet
				_context.Customers.Add(model);
				_context.SaveChanges();

				// Başarılı kayıt, Login sayfasına yönlendir
				return RedirectToAction("Login", "Customer");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Hata: {ex.Message}");
				ViewBag.ErrorMessage = "Kayıt işlemi sırasında bir hata oluştu.";
				return View(model);
			}
		}


	}
}
