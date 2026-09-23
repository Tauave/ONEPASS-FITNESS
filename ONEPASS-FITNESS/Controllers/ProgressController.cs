using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Services;
using System.Security.Claims;

namespace ONEPASS_FITNESS.Controllers
{
    public class ProgressController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgressController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Challenge();
            var model = await BuildIndexViewModelAsync(new WeightEntry
            {
                Date = DateOnly.FromDateTime(DateTime.Today)
            });
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("NewEntry")] ProgressIndexViewModel viewModel)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Challenge();

            var newEntry = viewModel.NewEntry;

            if (!ModelState.IsValid)
            {
                var invalidModel = await BuildIndexViewModelAsync(newEntry);
                return View(invalidModel);
            }

            //Stops user from entering a progress entry with a future date
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (newEntry.Date > today)
            {
                ModelState.AddModelError("NewEntry.Date", "Date cannot be in the future.");
                var invalidModel = await BuildIndexViewModelAsync(newEntry);
                return View(invalidModel);
            }

            //Users cannot add progress entries older than 3 months from the current date
            var minAllowed = DateOnly.FromDateTime(DateTime.Today.AddMonths(-3));
            if (newEntry.Date < minAllowed)
            {
                ModelState.AddModelError("NewEntry.Date", "You can only add progress within the last 3 months.");
                var invalidModel = await BuildIndexViewModelAsync(newEntry);
                return View(invalidModel);
            }

            newEntry.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            _context.WeightEntries.Add(newEntry);
            try
            {
                await _context.SaveChangesAsync();
            }
            //Stop users from entering multiple progress entries for the same date
            catch (DbUpdateException)
            {
                ModelState.AddModelError("NewEntry.Date", "You already logged weight for this date.");
                var invalidModel = await BuildIndexViewModelAsync(newEntry);
                return View(invalidModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Goal()
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Challenge();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var goal = await _context.WeightGoals.FirstOrDefaultAsync(g => g.UserId == userId);
            return View(goal ?? new WeightGoal());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Goal([Bind("StartWeightKg,TargetWeightKg,TargetDate")] WeightGoal model)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Challenge();

            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var existing = await _context.WeightGoals.FirstOrDefaultAsync(g => g.UserId == userId);

            if (existing == null)
            {
                model.UserId = userId;
                _context.WeightGoals.Add(model);
            }
            else
            {
                existing.StartWeightKg = model.StartWeightKg;
                existing.TargetWeightKg = model.TargetWeightKg;
                existing.TargetDate = model.TargetDate;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<ProgressIndexViewModel> BuildIndexViewModelAsync(WeightEntry newEntry)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var entries = await _context.WeightEntries
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.Date)
                .ToListAsync();

            var goal = await _context.WeightGoals.FirstOrDefaultAsync(g => g.UserId == userId);

            ProgressSummary? summary = null;
            IReadOnlyList<(DateOnly Date, decimal AverageKg)> movingAverage = Array.Empty<(DateOnly, decimal)>();

            if (entries.Count > 0)
            {
                movingAverage = ProgressCalculator.MovingAverage7Day(entries);
                if (goal != null)
                    summary = ProgressCalculator.Calculate(goal, entries);
            }

            return new ProgressIndexViewModel
            {
                NewEntry = newEntry,
                Entries = entries,
                Goal = goal,
                Summary = summary,
                MovingAverage = movingAverage
            };
        }
    }
}
