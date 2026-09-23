using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class WeightGoal
    {
        public int Id {get; set;}

        public string UserId {get; set;} = "";

        [Required]
        [Range(20, 500)]
        [Display(Name = "Starting weight (kg)")]
        public decimal StartWeightKg {get; set;}

        [Required]
        [Range(20, 500)]
        [Display(Name = "Target weight (kg)")]
        public decimal TargetWeightKg {get; set;}

        [DataType(DataType.Date)]
        [Display(Name = "Target date (optional)")]
        public DateOnly? TargetDate {get; set;}
    }
}
