namespace ONEPASS_FITNESS.Models
{
    public class ClassTypeListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; }
        public int UpcomingSessionCount { get; set; }
        public int TotalSessionCount { get; set; }
    }
}
