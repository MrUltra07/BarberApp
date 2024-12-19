using BarberApp.Models;
using System.ComponentModel.DataAnnotations;

namespace BarberApp.ViewModels;
public class EmployeeViewModel
{
    public Employee Employee { get; set; }
    public List<Skill> Skills { get; set; }

    [Required(ErrorMessage = "Please select at least one skill.")]
    public List<int> SelectedSkillIds { get; set; } = new List<int>();
}
