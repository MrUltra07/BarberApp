using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberApp.Controllers.Api
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Belirli bir beceri (skill) için çalışanları getirir.
        /// </summary>
        /// <param name="skillId">Beceri (skill) ID</param>
        /// <returns>Beceriye sahip çalışanların listesi</returns>
        [HttpGet]
        public IActionResult GetEmployeesBySkill([FromQuery] int skillId)
        {
            // Beceri kontrolü
            var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
            if (skill == null)
                return NotFound($"Skill with ID {skillId} not found.");

            // Beceriye sahip çalışanları LINQ ile getir
            var employees = _context.Employees
                .Include(e => e.Skills)
                .Where(e =>
                    e.Skills.Any(s => s.Id == skillId) &&                // Belirtilen beceriye sahip olmalı
                    e.Skills.Any(s => s.Title != "ADMIN")               // En az bir "ADMIN" olmayan beceriye sahip olmalı
                )
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Surname,
                    e.ProfileImageUrl,
                    Skills = e.Skills
                        .Where(s => s.Title != "ADMIN")                 // "ADMIN" olmayan beceriler
                        .Select(s => new { s.Id, s.Title })
                })
                .ToList();

            // Eğer çalışan yoksa, uygun mesaj döndür
            if (!employees.Any())
                return NotFound("No employees found with the specified skill.");

            // Çalışanları döndür
            return Ok(employees);
        }
    }
}
