using System.ComponentModel.DataAnnotations;

namespace FundooNotes.Models.DTOs.NoteDTOs
{
    public class UpdateNoteDTO
    {
        [Required(ErrorMessage = "Note Id is required")]
        public int id { get; set; }  

        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }  

        public string? Description { get; set; }

        public string? Color { get; set; }

        public string? Image { get; set; }

        public DateTime? Reminder { get; set; }
        public bool? IsArchive { get; set; }   

        public bool? IsTrash { get; set; }
        public bool? IsPinned { get; set; }
    }
}
