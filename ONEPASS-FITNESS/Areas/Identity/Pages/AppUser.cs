
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
        //[AgeValidation]
        public DateOnly DOB { get; set; }

        // Custom validation attribute to check if the user is at least 16 years old used in the DOB property
        //If the user is under 16, a validation error message will be displayed.
        public class AgeValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is DateOnly DOB)
                {
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    //Calcylate the age based on the Date of Birth and todays date
                    var age = today.Year - DOB.Year;
                    //If the user has not had their birthday yet this year, subtract 1 from the age
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
        public string PhoneNumber {get; set;}

        public ICollection<ClassBookings> ClassBookings {get; set;} = new List<ClassBookings>();

        public ICollection<Progress> Progress {get; set;} = new List<Progress>();

        public ICollection<WeightEntry> WeightEntries {get; set;} = new List<WeightEntry>();

        public ICollection<WeightGoal> WeightGoals {get; set;} = new List<WeightGoal>();

    }
}
