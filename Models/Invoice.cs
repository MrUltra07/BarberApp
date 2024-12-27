namespace BarberApp.Models
{
    public class Invoice
    {
        public  int Id { get; set; } // PK, AI
        public required int AppointmentId { get; set; } // FK
        public required decimal TipAmount { get; set; }

        public required Appointment Appointment { get; set; } // Navigation Property
    }
}
