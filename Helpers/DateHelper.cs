namespace BarberApp.Helpers
{
    public static class DateHelper
    {
        public static string GetDayName(int dayIndex)
        {
            string[] days = { "Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi" };
            return dayIndex >= 0 && dayIndex < days.Length ? days[dayIndex] : "Bilinmeyen Gün";
        }
    }
}
