using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ONEPASS_FITNESS.Models
{
    public class ClassSession
    {
        public int Id {get; set;}

        [Required]
        public int ClassTypeId {get; set;}
        public ClassType ClassType {get; set;} = null!;

        [Required]
        public DateTime StartTime {get; set;}

        [Range(1, 500)]
        public int Capacity {get; set;}

        public List<Booking> Bookings {get; set;}


    }
}
