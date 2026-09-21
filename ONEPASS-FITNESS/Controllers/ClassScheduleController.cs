using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;
using System.Security.Claims;

namespace ONEPASS_FITNESS.Controllers
{
    [Authorize(Policy = "MemberOnly")]
    public class ClassScheduleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ClassScheduleController> _logger;
        private readonly TimeZoneInfo _tz;

        public ClassScheduleController(ApplicationDbContext db, ILogger<ClassScheduleController> logger, TimeZoneInfo tz)
        {
            _db = db;
            _logger = logger;
            _tz = tz;
        }

        // GET: /ClassSchedule?classTypeId=1
        public async Task<IActionResult> Index(int? classTypeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var classTypes = await _db.ClassTypes
                .Where(ct => ct.IsActive)
                .OrderBy(ct => ct.Name)
                .ToListAsync();

            var query = _db.ClassSessions
                .Include(s => s.ClassType)
                .Include(s => s.Bookings)
                .Where(s => s.StartTime > DateTime.UtcNow && s.ClassType.IsActive);

            if (classTypeId != null)
                query = query.Where(s => s.ClassTypeId == classTypeId.Value);

            var sessions = await query.OrderBy(s => s.StartTime).ToListAsync();

            var bookedSessionIds = sessions
                .Where(s => s.Bookings.Any(b => b.UserId == userId))
                .Select(s => s.Id)
                .ToHashSet();

            ViewData["ClassTypes"] = classTypes;
            ViewData["SelectedClassTypeId"] = classTypeId;
            ViewData["BookedSessionIds"] = bookedSessionIds;

            return View(sessions);
        }

        // GET: /ClassSchedule/MySchedule
        public async Task<IActionResult> MySchedule()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var upcomingBookings = await _db.Bookings
                .Include(b => b.ClassSession)
                    .ThenInclude(s => s.ClassType)
                .Where(b => b.UserId == userId && b.ClassSession.StartTime > DateTime.UtcNow)
                .OrderBy(b => b.ClassSession.StartTime)
                .ToListAsync();

            var pastBookings = await _db.Bookings
                .Include(b => b.ClassSession)
                    .ThenInclude(s => s.ClassType)
                .Where(b => b.UserId == userId && b.ClassSession.StartTime <= DateTime.UtcNow)
                .OrderByDescending(b => b.ClassSession.StartTime)
                .ToListAsync();

            var vm = new MyScheduleViewModel
            {
                UpcomingBookings = upcomingBookings,
                PastBookings = pastBookings,
                TimeZone = _tz
            };

            return View(vm);
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

            return RedirectToAction(nameof(MySchedule));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int bookingId)
        {
            if (!User.Identity?.IsAuthenticated ?? true) return Challenge();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var booking = await _db.Bookings
                .Include(b => b.ClassSession)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction(nameof(MySchedule));
            }

            // Block cancellations within 2 hours of start time
            if (booking.ClassSession.StartTime <= DateTime.UtcNow.AddHours(2))
            {
                TempData["Error"] = "Cancellations are blocked within 2 hours of the session start.";
                return RedirectToAction(nameof(MySchedule));
            }

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Booking cancelled.";
            return RedirectToAction(nameof(MySchedule));
        }
    }

    public class MyScheduleViewModel
    {
        public List<Booking> UpcomingBookings { get; set; } = new();
        public List<Booking> PastBookings { get; set; } = new();
        public TimeZoneInfo TimeZone { get; set; } = null!;
    }
}

