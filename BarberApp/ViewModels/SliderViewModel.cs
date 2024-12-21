namespace BarberApp.ViewModels
{
	public class SliderViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		// Formdan gelen dosya
		public IFormFile ImageFile { get; set; }
	}
}
