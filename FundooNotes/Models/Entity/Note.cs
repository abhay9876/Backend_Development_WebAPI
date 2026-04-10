using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundooNotes.Models.Entity
{
    [Table("Note")]
    public class Note
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }
        public string? Color { get; set; }

        public string? Image { get; set; }

        public bool IsArchive { get; set; } = false;

        public bool IsTrash { get; set; } = false;

        public bool IsPinned { get; set; } = false;

        public DateTime? Reminder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

    }
}
