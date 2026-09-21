using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSessionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly TimeZoneInfo _tz;

        public AdminSessionsController(ApplicationDbContext db, TimeZoneInfo tz)
        {
            _db = db;
            _tz = tz;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _db.ClassSessions
                .Include(s => s.ClassType)
                .Include(s => s.Bookings)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new SessionFormViewModel()
            {
                RepeatWeeks = 1
            };
            await FillTypesAsync(vm);
            vm.StartLocal = DateTime.SpecifyKind(DateTime.Now.AddDays(1).Date.AddHours(9), DateTimeKind.Unspecified);
            return View("Form", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SessionFormViewModel vm)
        {
            await FillTypesAsync(vm);
            if (vm.StartLocal <= DateTime.Now)
                ModelState.AddModelError(nameof(vm.StartLocal), "Start time must be in the future.");

            if (vm.RepeatWeeks < 1 || vm.RepeatWeeks > 12)
                ModelState.AddModelError(nameof(vm.RepeatWeeks), "Repeat weeks must be between 1 and 12.");

            if (!ModelState.IsValid) return View("Form", vm);

            // Create one session per week
            for (int week = 0; week < vm.RepeatWeeks; week++)
            {
                // Calculate the date for this week in local time
                var localDateForWeek = vm.StartLocal.AddDays(week * 7);

                // Skip if this week is in the past
                if (localDateForWeek < DateTime.Now)
                    continue;

                // Convert this specific local date/time to UTC
                var startUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localDateForWeek, DateTimeKind.Unspecified), _tz);

                var s = new ClassSession
                {
                    ClassTypeId = vm.ClassTypeId,
                    StartTime = startUtc,
                    Capacity = vm.Capacity
                };
                _db.ClassSessions.Add(s);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var s = await _db.ClassSessions.Include(x => x.Bookings).FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            var vm = new SessionFormViewModel
            {
                Id = s.Id,
                ClassTypeId = s.ClassTypeId,
                Capacity = s.Capacity,
                StartLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(s.StartTime, DateTimeKind.Utc), _tz)
            };
            await FillTypesAsync(vm);
            return View("Form", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SessionFormViewModel vm)
        {
            var s = await _db.ClassSessions.Include(x => x.Bookings).FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            await FillTypesAsync(vm);

            if (vm.Capacity < s.Bookings.Count)
                ModelState.AddModelError(nameof(vm.Capacity), $"{s.Bookings.Count} people are already booked in.");

            if (!ModelState.IsValid) return View("Form", vm);

            s.ClassTypeId = vm.ClassTypeId;
            s.Capacity = vm.Capacity;
            s.StartTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(vm.StartLocal, DateTimeKind.Unspecified), _tz);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _db.ClassSessions.Include(x => x.Bookings).FirstOrDefaultAsync(x => x.Id == id);
            if (s == null) return NotFound();

            if (s.Bookings.Any())
            {
                TempData["Error"] = "That session has bookings. Ask members to cancel first.";
            }
            else
            {
                _db.ClassSessions.Remove(s);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task FillTypesAsync(SessionFormViewModel vm) =>
            vm.ClassTypes = await _db.ClassTypes
                .Where(t => t.IsActive)
                .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
                .ToListAsync();

        public class SessionFormViewModel
        {
            public int? Id { get; set; }

            [BindProperty]
            public int ClassTypeId { get; set; }

            [BindProperty]
            public DateTime StartLocal { get; set; }

            [BindProperty]
            public int Capacity { get; set; } = 15;

            [BindProperty]
            public int RepeatWeeks { get; set; } = 1;

            public List<SelectListItem> ClassTypes { get; set; } = new();
        }
    }
}
