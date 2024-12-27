using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using BarberApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BarberApp.Controllers.Admin
{
	[Route("admin")]
	public class SkilllController : Controller
	{
		private readonly AppDbContext _context;
		private static readonly string  viewAdress = "~/Views/Admin/Skill/";
		public SkilllController(AppDbContext context)
		{
			_context = context;
		}

		
		[HttpGet("skills")]
		public IActionResult Skills()
		{
			var skills = _context.Skills.ToList(); // Tüm skill'leri getir
			return View(viewAdress + "Index.cshtml", skills); // Skills.cshtml'e skill'leri gönder
		}

		[HttpGet("skills/create")]
		public IActionResult CreateSkill()
		{
			return View(viewAdress + "Create/Index.cshtml"); // CreateSkill.cshtml dosyasını döner
		}
		[HttpPost("skills/create")]
		public IActionResult CreateSkill(Skill newSkill)
		{
			if (ModelState.IsValid)
			{
				_context.Skills.Add(newSkill);
				_context.SaveChanges();
				return RedirectToAction("skills"); // Skill listeleme sayfasına yönlendir
			}

			return View(viewAdress + "Create/Index.cshtml", newSkill); // Model hatalıysa formu tekrar göster
		}

		[HttpGet("skills/edit/{id}")]
		public IActionResult EditSkill(int id)
		{
			var skill = _context.Skills.FirstOrDefault(s => s.Id == id);
			if (skill == null)
			{
				return NotFound(); // Eğer ID bulunamazsa 404 döndür
			}
			return View(viewAdress + "Edit/Index.cshtml", skill); // EditSkill.cshtml'e skill'i gönder
		}


		[HttpPost("Skills/Edit/{id}")]
		public IActionResult EditSkill(int id, Skill updatedSkill)
		{
			var skill = _context.Skills.FirstOrDefault(s => s.Id == id);
			if (skill == null)
			{
				return NotFound(); // Eğer ID bulunamazsa 404 döndür
			}

			// Skill'i güncelle
			skill.Title = updatedSkill.Title;
			skill.Description = updatedSkill.Description;
			skill.Price = updatedSkill.Price;
			skill.Bonus = updatedSkill.Bonus;
			skill.Duration = updatedSkill.Duration;
			skill.Cost = updatedSkill.Cost;
			skill.IsVisible = updatedSkill.IsVisible;

			_context.SaveChanges(); // Değişiklikleri kaydet

			return RedirectToAction("skills"); // Listeleme sayfasına yönlendir
		}
	}
}