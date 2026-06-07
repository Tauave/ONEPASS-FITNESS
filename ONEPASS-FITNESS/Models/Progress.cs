using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONEPASS_FITNESS.Models
{
    public class Progress
    {
        public int ProgressId { get; set; }

        [Required]
        [ForeignKey(nameof(Personalinfo))]
        public int Personalinfoid { get; set; }

        public Personalinfo Personalinfo { get; set; } 

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Weight { get; set; }

        [Required]
        public DateOnly DateRecorded { get; set; }
    }
}
