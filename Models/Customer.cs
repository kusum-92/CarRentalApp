// Models/Customer.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApp.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Name { get; set; } = "";

        [EmailAddress, MaxLength(120)]
        public string? Email { get; set; }

        [Phone, MaxLength(20)]
        public string? Phone { get; set; }

        [Required, MaxLength(30)]
        public string LicenseNumber { get; set; } = "";

        public string UserId { get; set; } = "";

        //navigation
       
        public IdentityUser? User { get; set; }
    }


}
