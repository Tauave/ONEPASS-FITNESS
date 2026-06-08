using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class Progress
    {
        public int ProgressId { get; set; }

        [Required]
        public int Personalinfoid { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Weight { get; set; }

        [Required]
        public DateOnly DateRecorded { get; set; }

        public Personalinfo Personalinfo { get; set; }
    }
}
