namespace BarberApp.Models
{
    public class Customer
    {
        public int Id { get; set; } // PK, AI
        public required string Name { get; set; } // *
        public required string Surname { get; set; } // *
        public required string Email { get; set; } // *
        public required string PhoneNumber { get; set; } // *
        public required DateTime BirthDay { get; set; } // *
		public required string Password { get; set; } // *
	}
}