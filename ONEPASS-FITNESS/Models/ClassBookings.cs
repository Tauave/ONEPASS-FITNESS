using ONEPASS_FITNESS.Areas.Identity.Pages;
using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class ClassBookings
    {
        [Key]
        public int BookingID { get; set; }

        [Required]
        public int Classid { get; set; }

        public Classes Class { get; set; }

        [Required]
        public int Personalinfoid { get; set; }

        [Required]
        public DateOnly BookingDate { get; set; }

        [Required]
        public string AttendanceStatus { get; set; }

        public AppUser appUser { get; set; }
    }
}
