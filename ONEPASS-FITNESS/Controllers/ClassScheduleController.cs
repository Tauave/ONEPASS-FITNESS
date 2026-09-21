using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;
using System.Security.Claims;

namespace ONEPASS_FITNESS.Controllers
{
    public class ClassScheduleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ClassScheduleController> _logger;

        public ClassScheduleController(ApplicationDbContext db, ILogger<ClassScheduleController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: /ClassSchedule?classTypeId=1
        public async Task<IActionResult> Index(int? classTypeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var classTypes = await _db.ClassTypes.ToListAsync();

            var query = _db.ClassSessions
                .Include(s => s.ClassType)
                .Include(s => s.Bookings)
                .Where(s => s.StartTime > DateTime.UtcNow);

            if (classTypeId != null)
                query = query.Where(s => s.ClassTypeId == classTypeId.Value);

            var sessions = await query.OrderBy(s => s.StartTime).ToListAsync();

            var bookedSessionIds = sessions
                .Where(s => s.Bookings.Any(b => b.UserId == userId))
                .Select(s => s.Id)
                .ToHashSet();

            // upcoming bookings for the current user 
            var upcomingBookings = await _db.Bookings
                .Include(b => b.ClassSession)
                    .ThenInclude(s => s.ClassType)
                .Where(b => b.UserId == userId && b.ClassSession.StartTime > DateTime.UtcNow)
                .OrderBy(b => b.ClassSession.StartTime)
                .ToListAsync();

            ViewData["UpcomingBookings"] = upcomingBookings;

            ViewData["ClassTypes"] = classTypes;
            ViewData["SelectedClassTypeId"] = classTypeId;
            ViewData["BookedSessionIds"] = bookedSessionIds;

            return View(sessions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int sessionId, int? classTypeId)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Challenge();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var session = await _db.ClassSessions
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null || session.StartTime <= DateTime.UtcNow)
            {
                TempData["Error"] = "Session not found or already started.";
                return RedirectToAction(nameof(Index), new { classTypeId });
            }

            if (session.Bookings.Any(b => b.UserId == userId))
            {
                TempData["Error"] = "You're already booked into this class.";
                return RedirectToAction(nameof(Index), new { classTypeId });
            }

            if (session.Bookings.Count >= session.Capacity)
            {
                TempData["Error"] = "Sorry, that class is full.";
                return RedirectToAction(nameof(Index), new { classTypeId });
            }

            var booking = new Booking { ClassSessionId = sessionId, UserId = userId };
            _db.Bookings.Add(booking);

            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = "You're booked in.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, "Booking failed for session {SessionId} user {UserId}", sessionId, userId);
                TempData["Error"] = "Could not complete booking. The class may be full or you already booked.";
            }

            return RedirectToAction(nameof(Index), new { classTypeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int bookingId, int? classTypeId)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Challenge();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var booking = await _db.Bookings
                .Include(b => b.ClassSession)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction(nameof(Index), new { classTypeId });
            }

            // Optionally block cancellations within 2 hours of start time
            if (booking.ClassSession.StartTime <= DateTime.UtcNow.AddHours(2))
            {
                TempData["Error"] = "Cancellations are blocked within 2 hours of the session start.";
                return RedirectToAction(nameof(Index), new { classTypeId });
            }

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Booking cancelled.";
            return RedirectToAction(nameof(Index), new { classTypeId });
        }
    }
}
