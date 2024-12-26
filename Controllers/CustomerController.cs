using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;


namespace BarberApp.Controllers
{

	public class CustomerController : Controller
	{
		private readonly AppDbContext _context;

		private bool IsUserLoggedIn()
		{
			return !string.IsNullOrEmpty(HttpContext.Session.GetString("CustomerId"));
		}


		public CustomerController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Login()
		{
			if (IsUserLoggedIn())
			{
				return RedirectToAction("Index", "Home");
			}
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
					ViewBag.ErrorMessage = "Bu e-posta adresine ait bir hesap bulunamadı.";
					return View("Login/Index");
				}

				if (customer.Password != password)
				{
					ViewBag.ErrorMessage = "Geçersiz şifre. Lütfen tekrar deneyin.";
					return View("Login/Index");
				}

				HttpContext.Session.SetString("CustomerName", customer.Name);
				HttpContext.Session.SetString("CustomerId", customer.Id.ToString());

				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
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
			if (IsUserLoggedIn())
			{
				return RedirectToAction("Index", "Home");
			}
			return View("Register/Index");
		}

		[HttpPost]
		public IActionResult Register(Customer model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.ErrorMessage = "Lütfen tüm alanları doğru doldurun.";
				return View("Register/Index");
			}

			if (_context.Customers.Any(c => c.Email == model.Email))
			{
				ViewBag.ErrorMessage = "Bu e-posta adresi zaten kullanılıyor.";
				return View("Register/Index");
			}

			_context.Customers.Add(model);
			_context.SaveChanges();

			return RedirectToAction("Login");
		}


	}
}
