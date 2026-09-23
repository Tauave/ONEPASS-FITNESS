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

        // GET: /AdminSessions
        public async Task<IActionResult> Index()
        {
            var sessions = await _db.ClassSessions
                .Include(s => s.ClassType)
                .Include(s => s.Bookings)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            ViewData["TimeZone"] = _tz;
            return View(sessions);
        }

        // GET: /AdminSessions/Create
        public async Task<IActionResult> Create()
        {
            var model = new ClassSessionViewModel
            {
                StartTime = ToLocal(DateTime.UtcNow).Date.AddDays(1).AddHours(9)
            };

            await PopulateClassTypesAsync(model.ClassTypeId);
            return View(model);
        }

        // POST: /AdminSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassSessionViewModel model)
        {
            if (!await _db.ClassTypes.AnyAsync(ct => ct.Id == model.ClassTypeId && ct.IsActive))
            {
                ModelState.AddModelError(nameof(model.ClassTypeId), "Pick an active class.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateClassTypesAsync(model.ClassTypeId);
                return View(model);
            }

            // Get the current UTC time to compare against the session start times
            // This ensures that we don't create sessions in the past
            //Weeks in the past will be skipped instead of being saved
            var now = DateTime.UtcNow;
            var created = 0;
            var skipped = 0;

            //Loop through the number of weeks to repeat the session and create a new ClassSession for each week
            for (var week = 0; week < model.RepeatWeeks; week++)
            {
                // Convert each week's local time separately so a daylight saving
                // change does not shift the time of day.
                var startUtc = ToUtc(model.StartTime.AddDays(7 * week));

                //Skip weeks that have already passed and do not create a session for them so the admin does not have to manually delete them later
                if (startUtc <= now)
                {
                    skipped++;
                    continue;
                }

                _db.ClassSessions.Add(new ClassSession
                {
                    ClassTypeId = model.ClassTypeId,
                    StartTime = startUtc,
                    Capacity = model.Capacity
                });

                created++;
            }

            if (created == 0)
            {
                ModelState.AddModelError(nameof(model.StartTime), "All of those dates are in the past.");
                await PopulateClassTypesAsync(model.ClassTypeId);
                return View(model);
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = skipped > 0
                ? $"Created {created} session(s). Skipped {skipped} week(s) already in the past."
                : $"Created {created} session(s).";

            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminSessions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _db.ClassSessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            var model = new ClassSessionViewModel
            {
                Id = session.Id,
                ClassTypeId = session.ClassTypeId,
                StartTime = ToLocal(session.StartTime),
                Capacity = session.Capacity
            };

            await PopulateClassTypesAsync(model.ClassTypeId);
            return View(model);
        }

        // POST: /AdminSessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassSessionViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var session = await _db.ClassSessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            var classTypeAllowed = await _db.ClassTypes
                .AnyAsync(ct => ct.Id == model.ClassTypeId && (ct.IsActive || ct.Id == session.ClassTypeId));

            if (!classTypeAllowed)
            {
                ModelState.AddModelError(nameof(model.ClassTypeId), "Pick an active class.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateClassTypesAsync(model.ClassTypeId, session.ClassTypeId);
                return View(model);
            }

            session.ClassTypeId = model.ClassTypeId;
            session.StartTime = ToUtc(model.StartTime);
            session.Capacity = model.Capacity;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Session updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminSessions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _db.ClassSessions
                .Include(s => s.ClassType)
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            ViewData["TimeZone"] = _tz;
            return View(session);
        }

        // POST: /AdminSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _db.ClassSessions
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                return NotFound();
            }

            _db.Bookings.RemoveRange(session.Bookings);
            _db.ClassSessions.Remove(session);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Session deleted.";
            return RedirectToAction(nameof(Index));
        }

        // Converts a local time to UTC, handling daylight saving time transitions
        private DateTime ToUtc(DateTime localTime)
        {
            var unspecified = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);

            if (_tz.IsInvalidTime(unspecified))
            {
                // Clocks jumped forward over this local time, shift past the gap.
                unspecified = unspecified.AddHours(1);
            }

            return TimeZoneInfo.ConvertTimeToUtc(unspecified, _tz);
        }

        private DateTime ToLocal(DateTime utcTime) =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcTime, DateTimeKind.Utc), _tz);

        private async Task PopulateClassTypesAsync(int selectedId, int? alwaysIncludeId = null)
        {
            var classTypes = await _db.ClassTypes
                .Where(ct => ct.IsActive || (alwaysIncludeId != null && ct.Id == alwaysIncludeId))
                .OrderBy(ct => ct.Name)
                .ToListAsync();

            ViewData["ClassTypes"] = new SelectList(classTypes, "Id", "Name", selectedId);
        }
    }
}
