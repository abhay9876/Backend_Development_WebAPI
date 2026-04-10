using System.ComponentModel.DataAnnotations;

namespace FundooNotes.Models.DTOs
{
    public class ResetPasswordDTO
    {
       

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}
