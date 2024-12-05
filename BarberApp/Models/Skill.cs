namespace BarberApp.Models
{
    public class Skill
    {
        public required int Id { get; set; } // PK, AI
        public required string Title { get; set; } // *
        public required string Description { get; set; } // *
        public required decimal Price { get; set; } // *
        public required decimal Bonus { get; set; } // *
        public required int Duration { get; set; } // *
        public required decimal Cost { get; set; } // *
        public required bool IsVisible { get; set; } // *
    }

}
