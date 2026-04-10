using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundooNotes.Models.Entity
{
    [Table("User")]
    public class User
    {
        [Key] // primary key
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]  // Validates email format
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }  

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }  

        public DateTime? DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  

        public DateTime? UpdatedAt { get; set; }

        //  for reset password
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

    }
}
