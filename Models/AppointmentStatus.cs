namespace BarberApp.Models
{
    public class AppointmentStatus
    {
        public required int Id { get; set; } // PK, AI
        public required string Name { get; set; } // *
        public required string Description { get; set; }
    }
}
