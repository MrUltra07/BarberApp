using BarberApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarberApp.Controllers.Admin
{
    [Route("admin/appointments")]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Available Slots
        [HttpGet("available-slots")]
        public IActionResult GetAvailableSlots(int skillId, int employeeId, DateTime date)
        {
            // Check if the skill exists for the employee
            var employee = _context.Employees
                .Include(e => e.Skills)
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null || !employee.Skills.Any(s => s.Id == skillId))
                return BadRequest("Skill not available for the specified employee.");

            // Get skill duration
            var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
            int skillDuration = skill.Duration;

            // Get employee's working hours for the given day
            int dayIndex = (int)date.DayOfWeek;
            var availability = _context.AvailableTimes.FirstOrDefault(a => a.DayIndex == dayIndex);
            if (availability == null)
                return BadRequest("No working hours found for this day.");

            // Get all appointments for the employee on the given date
            var appointments = _context.Appointments
                .Where(a => a.Employee.Id == employeeId && a.Date.Date == date.Date)
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
                // Check if the current slot is free
                bool isFree = appointments.All(appt =>
                    currentStartTime >= appt.EndTime ||
                    currentStartTime.Add(TimeSpan.FromMinutes(skillDuration)) <= appt.StartTime);

                if (isFree)
                    availableSlots.Add(currentStartTime);

                // Increment by 10 minutes
                currentStartTime = currentStartTime.Add(TimeSpan.FromMinutes(10));
            }

            return Ok(availableSlots.Select(t => t.ToString("hh\\:mm")));
        }

        [HttpPost("create")]
        public IActionResult CreateAppointment(int employeeId, int skillId, DateTime date, int? customerId = null)
        {
            // Kullanıcının rolünü veya kimliğini kontrol et
            if (HttpContext.Session.GetString("Role") == "Customer")
            {
                // Eğer giriş yapan bir müşteri ise, session'dan müşteri kimliğini alın
                var sessionCustomerId = HttpContext.Session.GetInt32("CustomerId");
                if (sessionCustomerId == null)
                    return Unauthorized("Customer session expired or not logged in.");

                customerId = sessionCustomerId.Value;
            }
            else if (HttpContext.Session.GetString("Role") != "Admin")
            {
                // Eğer giriş yapan admin değilse ve müşteri kimliği yoksa hata döndür
                return Unauthorized("You are not authorized to create appointments.");
            }

            // Müşteri kontrolü
            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
                return BadRequest("Invalid customer.");

            // Çalışan ve beceri kontrolü
            var employee = _context.Employees.Include(e => e.Skills).FirstOrDefault(e => e.Id == employeeId);
            var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);
            var status = _context.AppointmentStatuses.FirstOrDefault(s => s.Name == "Request");

            if (employee == null || skill == null || status == null)
                return BadRequest("Invalid employee, skill, or status.");

            if (!employee.Skills.Any(s => s.Id == skillId))
                return BadRequest("Skill not available for the specified employee.");

            // Randevu oluşturma
            var appointment = new Appointment
            {
                Date = date,
                Customer = customer,
                Employee = employee,
                Skill = skill,
                Status = status
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return Ok("Appointment created successfully.");
        }

        // POST: Update Appointment Status
        [HttpPost("update-status")]
        public IActionResult UpdateAppointmentStatus(int appointmentId, string statusName)
        {
            var appointment = _context.Appointments.Include(a => a.Status).FirstOrDefault(a => a.Id == appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found.");

            var newStatus = _context.AppointmentStatuses.FirstOrDefault(s => s.Name == statusName);
            if (newStatus == null)
                return BadRequest("Invalid status.");

            appointment.Status = newStatus;
            _context.SaveChanges();

            return Ok("Appointment status updated successfully.");
        }
    }
}
