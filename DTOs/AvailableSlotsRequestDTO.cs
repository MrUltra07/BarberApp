namespace BarberApp.DTOs;
public class AvailableSlotsRequestDTO
{
    public int SkillId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
}
