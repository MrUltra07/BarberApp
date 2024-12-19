using Microsoft.AspNetCore.Mvc;

namespace BarberApp.Controllers
{
	public class SliderController : Controller
	{
		public IActionResult Index()
		{
			// Görsel dosyalarının olduğu dizin
			string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

			// Görsellerin tam yollarını almak
			var imageFiles = Directory.GetFiles(imagesFolder)
									  .Where(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".webp") || file.EndsWith(".jpeg"))
									  .Select(file => "/images/" + Path.GetFileName(file))
									  .ToList();

			// Görselleri View'a iletmek
			return View(imageFiles);
		}
	}
}