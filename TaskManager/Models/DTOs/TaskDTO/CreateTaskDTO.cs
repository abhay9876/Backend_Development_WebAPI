using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.DTOs.TaskDTO
{
    public class CreateTaskDTO
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
    }


}
