using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using Microsoft.EntityFrameworkCore;
using BarberApp.ViewModels;

namespace BarberApp.Controllers.Admin
{
	[Route("admin")]
	public class SliderController : Controller
	{
		private readonly AppDbContext _context;
		private static readonly string viewAddress = "~/Views/Admin/Slider/";
		private readonly IWebHostEnvironment _env;


		public SliderController(AppDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;

		}

		[HttpGet("sliders")]
		public IActionResult Sliders()
		{
			var sliders = _context.Sliders.ToList(); // Get all sliders
			return View(viewAddress + "Index.cshtml", sliders); // Pass sliders to Index.cshtml
		}


		[HttpGet("sliders/create")]
		public IActionResult CreateSlider()
		{
			var viewModel = new SliderViewModel();
			return View("~/Views/Admin/Slider/Create/Index.cshtml", viewModel);
		}

		[HttpPost("sliders/create")]
		public async Task<IActionResult> CreateSlider(SliderViewModel model)
		{
			if (ModelState.IsValid)
			{
				// Dosya kontrolü ve kaydetme işlemi
				if (model.ImageFile != null && model.ImageFile.Length > 0)
				{
					var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
					var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
					var filePath = Path.Combine(uploadsFolder, fileName);

					// Dosyayı kaydediyoruz
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await model.ImageFile.CopyToAsync(stream);
					}

					// ImageUrl'i veritabanında kaydetmek için
					var slider = new Slider
					{
						Id = model.Id,
						Title = model.Title,
						Description = model.Description,
						ImageUrl = "/uploads/" + fileName // Dosya yolu veritabanına kaydediliyor
					};

					_context.Sliders.Add(slider);
					await _context.SaveChangesAsync();

					return RedirectToAction("Sliders");  // Sliders listesine yönlendirme
				}

				// Eğer dosya gelmediyse hata mesajı
				ModelState.AddModelError("ImageFile", "The image is required.");
			}

			// Model geçerli değilse, hatalarla birlikte formu tekrar döndürüyoruz
			return View("~/Views/Admin/Slider/Create/Index.cshtml", model);
		}

		




		[HttpGet("sliders/edit/{id}")]
		public IActionResult EditSlider(int id)
		{
			var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
			if (slider == null)
			{
				return NotFound(); // Return 404 if ID is not found
			}
			return View(viewAddress + "Edit/Index.cshtml", slider); // Pass slider to EditSlider.cshtml
		}

		[HttpPost("sliders/edit/{id}")]
		public IActionResult EditSlider(int id, IFormFile ImageFile, [FromForm] Slider updatedSlider)
		{
			var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
			if (slider == null)
			{
				return NotFound();
			}

			if (ImageFile != null && ImageFile.Length > 0)
			{
				// Save the file
				var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					ImageFile.CopyTo(stream);
				}

				// Delete the old image if necessary
				if (!string.IsNullOrEmpty(slider.ImageUrl))
				{
					var oldFilePath = Path.Combine(_env.WebRootPath, slider.ImageUrl.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}

				// Update the slider's image URL
				slider.ImageUrl = "/uploads/" + fileName;
			}

			// Update other fields
			slider.Title = updatedSlider.Title;
			slider.Description = updatedSlider.Description;

			_context.SaveChanges();

			return RedirectToAction("Sliders"); // Redirect back to the list of sliders
		}


		[HttpPost("sliders/delete/{id}")]
		public IActionResult DeleteSlider(int id)
		{
			var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);
			if (slider == null)
			{
				return NotFound();
			}

			// Eski dosya varsa sil
			if (!string.IsNullOrEmpty(slider.ImageUrl))
			{
				var filePath = Path.Combine(_env.WebRootPath, slider.ImageUrl.TrimStart('/'));
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}
			}

			_context.Sliders.Remove(slider);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

	}

}