using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberApp.Controllers.Api
{
    [Route("api/schedules")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScheduleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("available-slots")]
        public IActionResult GetAvailableSlots(int skillId, int employeeId, DateTime date)
        {
            // Tarihi UTC'ye çevir
            var utcDate = date.ToUniversalTime();

            // Validate the employee and skill relationship
            var employee = _context.Employees
                .Include(e => e.Skills)
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null || !employee.Skills.Any(s => s.Id == skillId))
                return BadRequest("Skill not available for the specified employee.");

            // Get skill duration
            var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
            int skillDuration = skill.Duration;

            // Get employee's working hours for the given day
            int dayIndex = (int)utcDate.DayOfWeek + 1;
            var availability = _context.AvailableTimes.FirstOrDefault(a => a.DayIndex == dayIndex);
            if (availability == null)
                return BadRequest("No working hours found for this day.");

            // Get all appointments for the employee on the given date
            var appointments = _context.Appointments
                .Where(a => a.Employee.Id == employeeId && a.Date.Date == utcDate.Date)
                .Select(a => new
                {
                    StartTime = a.Date.TimeOfDay,
                    EndTime = a.Date.TimeOfDay.Add(TimeSpan.FromMinutes(a.Skill.Duration))
                }).ToList();

            // Calculate available slots
            List<TimeSpan> availableSlots = new List<TimeSpan>();
            TimeSpan currentStartTime = availability.StartTime;

            while (currentStartTime.Add(TimeSpan.FromMinutes(skillDuration)) <= availability.EndTime)
            {
                // Check if the current slot is free or if there are no appointments
                bool isFree = appointments.All(appt =>
                    currentStartTime >= appt.EndTime ||
                    currentStartTime.Add(TimeSpan.FromMinutes(skillDuration)) <= appt.StartTime);

                if (isFree)
                    availableSlots.Add(currentStartTime);

                // Increment by 10 minutes
                currentStartTime = currentStartTime.Add(TimeSpan.FromMinutes(10));
            }

            // Return available slots
            if (!availableSlots.Any())
                return Ok(new List<string> { "No available slots for this day." });

            return Ok(availableSlots.Select(t => t.ToString("hh\\:mm")));
        }

    }
}
