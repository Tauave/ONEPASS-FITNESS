using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class WeightEntry
    {
        public int Id {get; set;}

        public string UserId {get; set;} = "";

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateOnly Date {get; set;}

        [Required]
        [Range(20, 500)]
        [Display(Name = "Weight (kg)")]
        public decimal WeightKg {get; set;}

        [Display(Name = "Note")]
        [MaxLength(500)]
        public string? Note {get; set;}
    }
}
