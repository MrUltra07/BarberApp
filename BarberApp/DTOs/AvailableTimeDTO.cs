namespace BarberApp.DTOs
{
    public class AvailableTimeDTO
    {
        public int DayIndex { get; set; }
        public string StartTime { get; set; } // Örn: "08:30"
        public string EndTime { get; set; }   // Örn: "17:00"
    }
}
