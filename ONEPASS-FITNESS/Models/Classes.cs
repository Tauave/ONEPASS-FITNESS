using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class Classes
    {
        [Key]
        public int Classid { get; set; }

        [Required]
        public string Classname { get; set; } 
        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeOnly Starttime { get; set; }

        [Required]
        public TimeOnly Endtime { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public ICollection<ClassBookings> ClassBookings { get; set; } = new List<ClassBookings>();
    }
}
