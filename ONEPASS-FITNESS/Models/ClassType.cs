using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Models
{
    public class ClassType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } 

        public string Description { get; set; }

        [Range(1, 1440)]
        public int DurationMinutes { get; set; }
    }
}
