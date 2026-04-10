using System.ComponentModel.DataAnnotations;

namespace FundooNotes.Models.DTOs
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
