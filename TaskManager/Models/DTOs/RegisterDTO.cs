using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.DTOs
{
    public class RegisterDTO
    {
       
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [MinLength(3)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
