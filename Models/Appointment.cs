using System.ComponentModel.DataAnnotations;

namespace BarberApp.Models
{
    public class Appointment
    {
        public  int Id { get; set; } // PK, AI
        public required DateTime Date { get; set; } // *

        public required Employee Employee { get; set; } // Navigation Property
        public required Customer Customer { get; set; } // Navigation Property
        public required Skill Skill { get; set; } // Navigation Property
        public required AppointmentStatus Status { get; set; } // Navigation Property
    }
}
