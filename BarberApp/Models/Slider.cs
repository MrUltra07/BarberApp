namespace BarberApp.Models
{
    public class Slider
    {
        public required  int Id { get; set; } // PK, AI
        public required string ImageUrl { get; set; } // *
        public required string Title { get; set; }
        public required string Description { get; set; }
    }

}
