using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class PersonalInfoViewModel
    {
        [Display(Name = "First name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Last name")]
        [Required]
        public string Lastname { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public string DOB { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
