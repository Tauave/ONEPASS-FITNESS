using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class ClassBookings
    {
        public int BookingID { get; set; }

        [Required]
        public int ClassId { get; set; }

        public Classes Class { get; set; }

        [Required]
        public int PersonalinfoId { get; set; }


        [Required]
        public DateOnly BookingDate { get; set; }

        [Required]
        public string AttendanceStatus { get; set; }

        public Personalinfo Personalinfo { get; set; }
    }
}
