using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class ClassTypeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Range(15, 180, ErrorMessage = "Duration must be between 15 and 180 minutes.")]
        public int DurationMinutes { get; set; } = 60;
    }
}
