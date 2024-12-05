namespace BarberApp.Models
{
    public class AvailableTime
    {
        public required int Id { get; set; } // PK, AI
        public required int DayIndex { get; set; } // *
        public required TimeSpan StartTime { get; set; } // *
        public required TimeSpan EndTime { get; set; } // *
    }

}
