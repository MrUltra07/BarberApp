namespace BarberApp.ViewModels
{
    public class AvailableTimeViewModel
    {
        public int DayIndex { get; set; } // Gün indeksi
        public string StartTime { get; set; } // Başlangıç zamanı (hh:mm)
        public string EndTime { get; set; } // Bitiş zamanı (hh:mm)
    }
}
