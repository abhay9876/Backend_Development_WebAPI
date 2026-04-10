using System.ComponentModel.DataAnnotations;

namespace FundooNotes.Models.DTOs.NoteDTOs
{
    public class CreateNoteDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Color { get; set; }

        public string? Image { get; set; }

        public DateTime? Reminder { get; set; }
    }
}
