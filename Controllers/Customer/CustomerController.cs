using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;


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
			return View("Login/Index"); 
		}

		[HttpPost]
		public IActionResult Login(string email, string password)
		{
			try
			{
				// Kullanıcıyı e-posta adresine göre bul
				var customer = _context.Customers.FirstOrDefault(c => c.Email == email);

				if (customer == null)
				{
					// Kullanıcı bulunamazsa
					ViewBag.ErrorMessage = "Bu e-posta adresine ait bir hesap bulunamadı.";
					return View("Login/Index");
				}

				// Şifreyi kontrol et (düz metin karşılaştırması)
				if (customer.Password != password)
				{
					// Şifre yanlışsa
					ViewBag.ErrorMessage = "Geçersiz şifre. Lütfen tekrar deneyin.";
					return View("Login/Index");
				}

				// Başarılı giriş
				HttpContext.Session.SetString("CustomerName", customer.Name);
				HttpContext.Session.SetString("CustomerId", customer.Id.ToString());

				// Ana sayfaya yönlendir
				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				// Hata durumunda logla
				ViewBag.ErrorMessage = "Bir hata oluştu. Lütfen tekrar deneyiniz.";
				Console.WriteLine($"Hata: {ex.Message}");
				return View("Login/Index");
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
			return View("Register/Index");
		}

		[HttpPost]
		public IActionResult Register(BarberApp.Models.Customer model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.ErrorMessage = "Lütfen tüm alanları doğru doldurun.";
				return View("Register/Index");
			}

			// Zaman türünü UTC'ye çevir
			model.BirthDay = DateTime.SpecifyKind(model.BirthDay, DateTimeKind.Utc);

			// E-posta adresinin benzersiz olup olmadığını kontrol et
			if (_context.Customers.Any(c => c.Email == model.Email))
			{
				ViewBag.ErrorMessage = "Bu e-posta adresi zaten kullanılıyor.";
				return View("Register/Index");
			}

			// Şifreyi hashle
			//var passwordHasher = new PasswordHasher<BarberApp.Models.Customer>();
			//model.Password = passwordHasher.HashPassword(model, model.Password);

			// Yeni kullanıcıyı kaydet
			_context.Customers.Add(model);
			_context.SaveChanges();

			// Başarılı kayıt sonrası giriş sayfasına yönlendir
			return RedirectToAction("Login");
		}

	}
}
