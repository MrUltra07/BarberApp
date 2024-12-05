namespace BarberApp.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<GeneralSettings> GeneralSettings { get; set; }
    public DbSet<AvailableTime> AvailableTimes { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<EmployeeSkillLinked> EmployeeSkillLinkeds { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
}
