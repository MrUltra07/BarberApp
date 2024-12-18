using Microsoft.EntityFrameworkCore;

namespace BarberApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<GeneralSettings> GeneralSettings { get; set; }
        public DbSet<AvailableTime> AvailableTimes { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Admin Employee
            modelBuilder.Entity<Employee>().HasData(new Employee
            {
                Id = 1,
                Name = "Admin",
                Surname = "User",
                ProfileImageUrl = "default.png",
                IdNumber = "00000000000",
                Iban = "TR000000000000000000000000",
                BasicWage = 5000,
                Password = "admin123"
            });

            // Admin Skill
            modelBuilder.Entity<Skill>().HasData(new Skill
            {
                Id = 1,
                Title = "ADMIN",
                Description = "Administrator Role",
                Price = 0,
                Bonus = 0,
                Duration = 0,
                Cost = 0,
                IsVisible = true
            });

            // Employee-Skill İlişkisi
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Skills)
                .WithMany(s => s.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeSkill",
                    r => r.HasOne<Skill>().WithMany().HasForeignKey("SkillId"),
                    l => l.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId"),
                    joinEntity =>
                    {
                        joinEntity.HasData(new { EmployeeId = 1, SkillId = 1 });
                    });
        }


    }
}
