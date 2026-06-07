using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Controllers
{
    [Authorize]
    public class ClassBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClassBookingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
            {
                return RedirectToAction("Create", "Personalinfo");
            }

            var bookings = await _context.ClassBookings
                .Include(b => b.Class)
                .Where(b => b.Personalinfoid == profile.PersonalinfoId)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> Create()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
            {
                return RedirectToAction("Create", "Personalinfo");
            }

            ViewBag.Profile = profile;
            return View(await _context.Classes.OrderBy(c => c.Date).ThenBy(c => c.Starttime).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int classId)
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
            {
                return RedirectToAction("Create", "Personalinfo");
            }

            var gymClass = await _context.Classes.FindAsync(classId);
            if (gymClass == null)
            {
                return NotFound();
            }

            var alreadyBooked = await _context.ClassBookings.AnyAsync(b =>
                b.Personalinfoid == profile.PersonalinfoId && b.Classid == classId);

            if (!alreadyBooked)
            {
                _context.ClassBookings.Add(new ClassBookings
                {
                    Classid = classId,
                    Personalinfoid = profile.PersonalinfoId,
                    BookingDate = DateOnly.FromDateTime(DateTime.Today),
                    AttendanceStatus = "Booked"
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<Personalinfo> GetCurrentProfileAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return null;
            }

            return await _context.Personalinfos
                .FirstOrDefaultAsync(p => p.IdentityUserId == userId);
        }
    }
}
