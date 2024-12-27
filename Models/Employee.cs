namespace BarberApp.Models
{
    public class Employee
    {
        public int Id { get; set; } // PK
        public required string Name { get; set; } // Zorunlu alan
        public required string Surname { get; set; } // Zorunlu alan
        public required string ProfileImageUrl { get; set; } // Zorunlu alan
        public required string IdNumber { get; set; } // Zorunlu alan
        public required string Iban { get; set; } // Zorunlu alan
        public required decimal BasicWage { get; set; } // Zorunlu alan
        public required string Password { get; set; } // Zorunlu alan

        // Employee'nin Skills ile ilişkisi
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
