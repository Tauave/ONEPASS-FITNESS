using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Services
{
    //Holds the summary of progress towards a weight goal so that it can be displayed in the UI
    public record ProgressSummary(
        decimal Current,
        decimal Change,
        decimal PercentToGoal,
        decimal Remaining);

    //Calculates how far along a user is towards their weight goal based on their weight entries
    //Weight loss and wegiht gain are both supported, and the progress is calculated as a percentage of the total weight change needed to reach the goal.
    public static class ProgressCalculator
    {
        public static ProgressSummary Calculate(WeightGoal goal, IReadOnlyList<WeightEntry> entries)
        {
            //Stops the calculation if there are no entries, and returns a summary with all values set to 0
            var current = entries.OrderBy(e => e.Date).Last().WeightKg;
            var totalNeeded = goal.TargetWeightKg - goal.StartWeightKg;
            var done = current - goal.StartWeightKg;
            //Stops a divide be zero if the total needed is 0
            var percent = totalNeeded == 0
                ? 100
                //Keeps the percentage between 0 and 100, even if the user has gone past their goal
                : Math.Clamp(done / totalNeeded * 100, 0, 100);

            return new ProgressSummary(current, done, percent, goal.TargetWeightKg - current);
        }

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
