namespace BarberApp.ViewModels
{
    public class AppointmentViewModel
    {
        public string MonthYear { get; set; }
        public decimal TotalIncome { get; set; }
        public int EmployeeId { get; set; }
        public bool IsAdmin { get; set; }
        public List<KazancDataViewModel> KazancData { get; set; } // KazancData'nın doğru türde olduğunu kontrol edin
    }

    public class KazancDataViewModel
    {
        public string SkillName { get; set; }
        public decimal Kazanc { get; set; }
        public string Date { get; set; }
        public string EmployeeName { get; set; }
    }



}
