using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class ClassSessionViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Class")]
        public int ClassTypeId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Start time")]
        public DateTime StartTime { get; set; }

        [Range(1, 500)]
        public int Capacity { get; set; } = 10;

        [Display(Name = "Repeat weekly for N weeks")]
        [Range(1, 12, ErrorMessage = "Repeat weeks must be between 1 and 12.")]
        public int RepeatWeeks { get; set; } = 1;
    }
}
