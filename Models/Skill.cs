namespace BarberApp.Models
{
    public class Skill
    {
        public int Id { get; set; } // PK
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Bonus { get; set; }
        public int Duration { get; set; }
        public decimal Cost { get; set; }
        public bool IsVisible { get; set; }

        // Employees ile ilişki
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
