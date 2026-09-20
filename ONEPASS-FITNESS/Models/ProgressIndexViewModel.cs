using ONEPASS_FITNESS.Services;

namespace ONEPASS_FITNESS.Models
{
    public class ProgressIndexViewModel
    {
        public WeightEntry NewEntry { get; set; } = new()
        {
            Date = DateOnly.FromDateTime(DateTime.Today)
        };

        public List<WeightEntry> Entries { get; set; } = new();

        public WeightGoal? Goal { get; set; }

        public ProgressSummary? Summary { get; set; }

        public IReadOnlyList<(DateOnly Date, decimal AverageKg)> MovingAverage { get; set; }
            = Array.Empty<(DateOnly, decimal)>();
    }
}
