namespace BarberApp.ViewModels
{
    public class GeneralSettingsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public IFormFile LogoFile { get; set; } // Dosya yükleme için
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string Keywords { get; set; }
    }
}
