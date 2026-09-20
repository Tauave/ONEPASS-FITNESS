using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Services
{
    public record ProgressSummary(
        decimal Current,
        decimal Change,
        decimal PercentToGoal,
        decimal Remaining);

    public static class ProgressCalculator
    {
        public static ProgressSummary Calculate(WeightGoal goal, IReadOnlyList<WeightEntry> entries)
        {
            var current = entries.OrderBy(e => e.Date).Last().WeightKg;
            var totalNeeded = goal.TargetWeightKg - goal.StartWeightKg;
            var done = current - goal.StartWeightKg;

            var percent = totalNeeded == 0
                ? 100
                : Math.Clamp(done / totalNeeded * 100, 0, 100);

            return new ProgressSummary(current, done, percent, goal.TargetWeightKg - current);
        }

        /// <summary>7-day moving average aligned to each entry date (inclusive window).</summary>
        public static IReadOnlyList<(DateOnly Date, decimal AverageKg)> MovingAverage7Day(IReadOnlyList<WeightEntry> entries)
        {
            if (entries.Count == 0)
                return Array.Empty<(DateOnly, decimal)>();

            var ordered = entries.OrderBy(e => e.Date).ToList();
            var result = new List<(DateOnly, decimal)>(ordered.Count);

            foreach (var entry in ordered)
            {
                var windowStart = entry.Date.AddDays(-6);
                var window = ordered.Where(e => e.Date >= windowStart && e.Date <= entry.Date).ToList();
                result.Add((entry.Date, window.Average(e => e.WeightKg)));
            }

            return result;
        }
    }
}
