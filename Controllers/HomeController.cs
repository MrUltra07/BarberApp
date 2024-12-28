using System.Diagnostics;
using BarberApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BarberApp.Controllers
{
	public class HomeController : BaseController
	{
		private readonly ILogger<HomeController> _logger;
		private readonly AppDbContext _context; // AppDbContext baðýmlýlýðý

		public HomeController(ILogger<HomeController> logger, AppDbContext context) : base(context)
		{
			_logger = logger;
			_context = context;
		}

		public IActionResult Index()
		{
			// Slider verilerini veritabanýndan çekiyoruz
			var sliders = _context.Sliders.ToList();

			// Slider verilerini ViewData ile View'a gönderiyoruz
			ViewData["Sliders"] = sliders;

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
