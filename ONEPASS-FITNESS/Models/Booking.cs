using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int ClassSessionId { get; set; }
        public ClassSession ClassSession { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    }
}
