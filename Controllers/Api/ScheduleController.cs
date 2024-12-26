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
            var laterDate = date.AddDays(+1);
            // 1. İstek ile gelen çalışan ve beceri eşleşmesini doğrula
            var employee = _context.Employees
                .Include(e => e.Skills)
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null || !employee.Skills.Any(s => s.Id == skillId))
                return BadRequest("Skill not available for the specified employee.");

            // 2. İstenilen beceri süresini al
            var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
            if (skill == null)
                return BadRequest("Skill not found.");
            int skillDuration = skill.Duration;

            // 3. Dinlenme süresini ata
            int breakTime = 10;

            // 4. Verilen date günündeki dükkanın açık kaldığı süreleri çek
            int dayIndex = (int)date.DayOfWeek;
            var availability = _context.AvailableTimes.FirstOrDefault(a => a.DayIndex == dayIndex);
            if (availability == null)
                return BadRequest("No working hours found for this day.");

            // 5. Bu başlangıç ve bitiş zamanı arasındaki tüm zamanı 10'ar dakikalık partlara böl ve bir listeye ata
            List<TimeSpan> allSlots = new List<TimeSpan>();
            TimeSpan currentSlot = availability.StartTime;

            while (currentSlot.Add(TimeSpan.FromMinutes(skillDuration)) <= availability.EndTime)
            {
                allSlots.Add(currentSlot);
                currentSlot = currentSlot.Add(TimeSpan.FromMinutes(10));
            }
            // DateTime'ı UTC+3 zaman dilimine dönüştür
            var utcDate = laterDate.ToUniversalTime();

            // Gün başlangıcını ve bitişini UTC olarak ayarla
            var startOfDayUtc = utcDate.Date;
            var endOfDayUtc = utcDate.Date.AddDays(1).AddSeconds(-1);

            // Randevuları çek
            var appointments = _context.Appointments
                .Where(a => a.Employee.Id == employeeId
                            && a.Date >= startOfDayUtc && a.Date <= endOfDayUtc) // UTC olarak tarih filtreleme
                .Select(a => new
                {
                    StartTime = a.Date.TimeOfDay,
                    EndTime = a.Date.TimeOfDay.Add(TimeSpan.FromMinutes(a.Skill.Duration)),
                    Duration = a.Skill.Duration,
                })
                .OrderBy(a => a.StartTime)
                .ToList();

            // 7. Randevuları döngüye sok
            foreach (var appointment in appointments)
            {
                // Randevunun başlangıç ve bitiş saatlerini UTC+3'e dönüştür
                var appointmentStartLocal = appointment.StartTime.Add(TimeSpan.FromHours(3));
                var appointmentEndLocal = appointment.EndTime.Add(TimeSpan.FromHours(3));

                // 8. Randevunun bitiş süresini hesapla + bekleme süresini ekle
                var appointmentEndWithBreak = appointmentEndLocal.Add(TimeSpan.FromMinutes(breakTime));

                // 9. Randevunun başlangıç saati ile bitiş + bekleme süresi arasındaki tüm 10'ar dakikalık süreleri listeden çıkart
                allSlots = allSlots.Where(slot =>
                    slot.Add(TimeSpan.FromMinutes(skillDuration)) <= appointmentStartLocal ||
                    slot >= appointmentEndWithBreak).ToList();

                // 10. İstenilen beceri süresini randevu başlangıcından çıkart ve aradaki tüm 10'ar dakikaları listeden çıkart
                //var appointmentStartMinusSkill = appointmentStartLocal.Subtract(TimeSpan.FromMinutes(skillDuration));
                allSlots = allSlots.Where(slot =>
                    slot.Add(TimeSpan.FromMinutes(skillDuration)).Add(TimeSpan.FromMinutes(10)) <= appointmentStartLocal ||
                    slot >= appointmentStartLocal).ToList();
            }


            // 11. Tüm randevular bitince döngüden çıkmış olacağız

            // 12. Listenin son halini API'de döndür
            if (!allSlots.Any())
                return Ok(new List<string> { "No available slots for this day." });

            //return Ok(new
            //{
            //    AvailableSlots = allSlots,
            //    Appointments = appointments.Select(a => new
            //    {
            //        StartTimeUtc = a.StartTime.ToString(@"hh\:mm"),
            //        EndTimeUtc = a.EndTime.ToString(@"hh\:mm"),
            //        StartTimeLocal = a.StartTime.Add(TimeSpan.FromHours(3)).ToString(@"hh\:mm"),
            //        EndTimeLocal = a.EndTime.Add(TimeSpan.FromHours(3)).ToString(@"hh\:mm")
            //    })
            //});
            return Ok(allSlots);
        }


    }
}