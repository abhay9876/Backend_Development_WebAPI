using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models.Entity
{
    [Table("User")]
    public class User
    {

        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [MinLength(3)]
        public string Password { get; set; }

        public string Role { get; set; } = "User";
    }
}
