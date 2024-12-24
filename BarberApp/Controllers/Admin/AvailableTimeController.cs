using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using BarberApp.ViewModels;
using System.Reflection.Metadata.Ecma335;

namespace BarberApp.Controllers.Admin
{
    [Route("admin")]
    public class AvailableTimeController : Controller
    {
        private readonly AppDbContext _context;

        public AvailableTimeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("availability")]
        public IActionResult Index()
        {
            var availableTimes = _context.AvailableTimes
                .OrderBy(at => at.DayIndex) // DayIndex'e göre küçükten büyüğe sıralama
                .ToList();
            return View("~/Views/Admin/AvailableTime/Index.cshtml", availableTimes);
        }



        [HttpPost("availability/update")]
        public IActionResult Update(AvailableTimeViewModel viewModel)
        {
            try
            {
                var availableTime = _context.AvailableTimes.FirstOrDefault(a => a.DayIndex == viewModel.DayIndex);
                if (availableTime == null)
                {
                    TempData["Status"] = "error";
                    TempData["Message"] = "Day not found.";
                    //return RedirectToAction("Hata");
                    return Ok("Route hata!");
                }

                availableTime.StartTime = TimeSpan.Parse(viewModel.StartTime);
                availableTime.EndTime = TimeSpan.Parse(viewModel.EndTime);

                _context.SaveChanges();

                TempData["Status"] = "ok";
                TempData["Message"] = "Working hours updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Status"] = "error";
                TempData["Message"] = $"Error updating working hours: {ex.Message}";
            }

            return RedirectToAction("Index");
            //return Ok("Denemee");
        }


    }
}
