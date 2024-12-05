namespace BarberApp.Models
{
    public class GeneralSettings
    {
        public required int Id { get; set; } // PK, AI
        public required string Name { get; set; } // *
        public required string Description { get; set; } // *
        public required string LogoUrl { get; set; }
        public required string SeoTitle { get; set; }
        public required string SeoDescription { get; set; }
        public required string Keywords { get; set; }
    }

}
