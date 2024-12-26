using System.ComponentModel.DataAnnotations;
namespace BarberApp.ViewModels;
public class BookAppointmentViewModel
{
    public int SkillId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime AppointmentDate { get; set; } // Tarih (gün) bilgisi

    [Required(ErrorMessage = "Lütfen uygun bir saat seçin.")]

    public string TimeSlot { get; set; } // Kullanıcının seçtiği saat
}
