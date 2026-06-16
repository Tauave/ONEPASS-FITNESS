
using Microsoft.AspNetCore.Identity;
using ONEPASS_FITNESS.Models;
using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Areas.Identity.Pages
{
    public class AppUser: IdentityUser  
    {
        [Required]
        public string Name { get; set; }


        [Required]
        public string Lastname { get; set; }

        

        [Required]
        [AgeValidation]
        public DateOnly DOB { get; set; }
        public class AgeValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is DateOnly DOB)
                {
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    var age = today.Year - DOB.Year;
                    if (DOB > today.AddYears(-age)) age--;
                    if (age < 16)
                    {
                        return new ValidationResult("You must be at least 16 years old.");
                    }
                }
                return ValidationResult.Success;
            }
        }


        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public ICollection<ClassBookings>? ClassBookings { get; set; } = new List<ClassBookings>();

        public ICollection<Progress>? Progress { get; set; } = new List<Progress>();

    }
}
