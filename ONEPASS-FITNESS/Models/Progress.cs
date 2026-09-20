using ONEPASS_FITNESS.Areas.Identity.Pages;
using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class Progress
    {

        public int ProgressId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Weight { get; set; }

        // store FK to the AppUser; keep navigation for queries only
        public string? AppUserId { get; set; }

        public AppUser? appUser { get; set; }

        [Required]
        public DateOnly DateRecorded { get; set; }
    }
}
