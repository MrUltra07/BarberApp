using System.ComponentModel.DataAnnotations;

namespace BarberApp.Models
{
    public class Slider
    {
        public required  int Id { get; set; } // PK, AI


		public required string ImageUrl { get; set; } // *

		[Required(ErrorMessage = "Title is required.")]
		public required string Title { get; set; }


		[Required(ErrorMessage = "Description is required.")]
		public required string Description { get; set; }
    }

}
