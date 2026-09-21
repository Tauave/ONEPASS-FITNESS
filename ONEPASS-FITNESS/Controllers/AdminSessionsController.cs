using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Services;

namespace ONEPASS_FITNESS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSessionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly GymTimeZoneProvider _timeZone;

        public AdminSessionsController(ApplicationDbContext db, GymTimeZoneProvider timeZone)
        {
            _db = db;
            _timeZone = timeZone;
        }

        // GET: /AdminSessions
        public async Task<IActionResult> Index()
        {
            var sessions = await _db.ClassSessions
                .Include(s => s.ClassType)
                .Include(s => s.Bookings)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            ViewData["TimeZone"] = _timeZone;
            return View(sessions);
        }

        // GET: /AdminSessions/Create
        public async Task<IActionResult> Create()
        {
            var model = new ClassSessionViewModel
            {
                StartTime = _timeZone.ToLocal(DateTime.UtcNow).Date.AddDays(1).AddHours(9)
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

            var now = DateTime.UtcNow;
            var created = 0;
            var skipped = 0;

            for (var week = 0; week < model.RepeatWeeks; week++)
            {
                // Convert each week's local time separately so a daylight saving
                // change does not shift the time of day.
                var startUtc = _timeZone.ToUtc(model.StartTime.AddDays(7 * week));

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
                StartTime = _timeZone.ToLocal(session.StartTime),
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
            session.StartTime = _timeZone.ToUtc(model.StartTime);
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

            ViewData["TimeZone"] = _timeZone;
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
