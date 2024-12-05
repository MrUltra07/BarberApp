namespace BarberApp.Models
{
    public class EmployeeSkillLinked
    {
        public required int Id { get; set; } // PK, AI
        public required int PersonelId { get; set; } // FK
        public required int SkillId { get; set; } // FK

        public required Employee Employee { get; set; } // Navigation Property
        public required Skill Skill { get; set; } // Navigation Property
    }
}
