// InvoiceController.cs
using Microsoft.AspNetCore.Mvc;
using BarberApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberApp.Controllers
{
    [Route("admin/invoice")]
    public class InvoiceController : Controller
    {
        private readonly AppDbContext _context;
        private static readonly string viewAdress = "~/Views/Admin/Invoice/";

        public InvoiceController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Employee)
                .Include(i => i.Appointment)
                .ThenInclude(a => a.Customer).OrderByDescending(a => a.Appointment.Date)
                .ToListAsync();

            return View(viewAdress + "Index.cshtml", invoices);
        }
        [HttpGet("create")]
        public IActionResult Create(int appointmentId)
        {
            // Appointment bilgilerini getir
            var appointment = _context.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Customer)
                .Include(a => a.Skill)
                .FirstOrDefault(a => a.Id == appointmentId);

            if (appointment == null)
                return NotFound("Appointment bulunamadı.");

            // Mevcut faturayı kontrol et
            var existingInvoice = _context.Invoices
                .FirstOrDefault(i => i.AppointmentId == appointmentId);

            if (existingInvoice != null)
            {
                // Zaten oluşturulmuş bir fatura varsa, Invoice listesinin olduğu sayfaya yönlendir
                return RedirectToAction("Index", "Invoice");
            }

            // View'a Appointment bilgilerini gönder
            return View(viewAdress + "Create/Index.cshtml", appointment);
        }

        [HttpPost("create")]
        public IActionResult Create(int appointmentId, decimal tipAmount)
        {
            // Appointment bilgilerini al
            var appointment = _context.Appointments
                .Include(a => a.Employee)
                .Include(a => a.Skill)
                .FirstOrDefault(a => a.Id == appointmentId);

            if (appointment == null)
                return NotFound("Appointment bulunamadı.");

            // Mevcut faturayı kontrol et
            var existingInvoice = _context.Invoices
                .FirstOrDefault(i => i.AppointmentId == appointmentId);

            if (existingInvoice != null)
            {
                // Zaten oluşturulmuş bir fatura varsa, Invoice listesinin olduğu sayfaya yönlendir
                return RedirectToAction("Index", "Invoice");
            }

            // Invoice oluştur
            var invoice = new Invoice
            {
                AppointmentId = appointmentId,
                TipAmount = tipAmount,
                Appointment = appointment,
            };

            _context.Invoices.Add(invoice);

            // Appointment durumunu güncelle
            appointment.Status = _context.AppointmentStatuses.FirstOrDefault(s => s.Name == "Completed");

            _context.SaveChanges();

            // İşlem sonrası yönlendirme
            return RedirectToAction("Index", "Invoice");
        }
    }
}
