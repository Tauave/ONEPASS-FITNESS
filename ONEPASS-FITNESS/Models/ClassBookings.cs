using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONEPASS_FITNESS.Models
{
    public class ClassBookings
    {
        public int BookingID { get; set; }

        [Required]
        [ForeignKey(nameof(Class))]
        public int Classid { get; set; }

        public Classes Class { get; set; } 

        [Required]
        [ForeignKey(nameof(Personalinfo))]
        public int Personalinfoid { get; set; }

        public Personalinfo Personalinfo { get; set; }

        [Required]
        public DateOnly BookingDate { get; set; }

        [Required]
        public string AttendanceStatus { get; set; } 
    }
}
