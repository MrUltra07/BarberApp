namespace BarberApp.Models
{
    public class Employee
    {
        public int Id { get; set; } // PK, AI
        public required string Name { get; set; } // *
        public required string Surname { get; set; } // *
        public required string ProfileImageUrl { get; set; } // *
        public required string IdNumber { get; set; } // *
        public required string Iban { get; set; } // *
        public required decimal BasicWage { get; set; } // *
        public required string Password { get; set; } // *
    }

}
