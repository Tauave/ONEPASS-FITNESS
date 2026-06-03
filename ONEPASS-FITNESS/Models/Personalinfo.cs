using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ONEPASS_FITNESS.Models
{
    public class Personalinfo
    {
        public int PersonalinfoId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        public DateOnly DOB { get; set; }

        [Required]
        public EmailAddressAttribute Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }


    }
}
