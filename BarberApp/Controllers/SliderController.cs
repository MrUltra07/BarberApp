using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BarberApp.Controllers
{
	public class SliderController : Controller
	{
		private readonly AppDbContext _context;

		public SliderController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var sliders = _context.Sliders.ToList(); // Veritabanındaki sliderları çekiyoruz

			// Eğer slider verisi bulunmazsa, boş bir liste gönderelim
			if (sliders == null)
			{
				sliders = new List<Slider>(); // Boş liste oluşturuyoruz
			}

			return View("Index",sliders); // Model'i view'a gönderiyoruz
		}

	}
}
