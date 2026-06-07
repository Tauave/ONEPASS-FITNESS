using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class Personalinfo
    {
        public int PersonalinfoId { get; set; }

        [Required]
        public string IdentityUserId { get; set; } 

        [Required]
        public string Name { get; set; } 

        [Required]
        public string Lastname { get; set; } 

        [Required]
        public DateOnly DOB { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } 

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } 

        public ICollection<ClassBookings> ClassBookings { get; set; } = new List<ClassBookings>();

        public ICollection<Progress> Progress { get; set; } = new List<Progress>();
    }
}
