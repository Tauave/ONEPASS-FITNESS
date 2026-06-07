using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Models.ViewModels;

namespace ONEPASS_FITNESS.Controllers
{
    [Authorize]
    public class PersonalinfoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PersonalinfoController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var bookings = await _context.ClassBookings
                .Include(b => b.Class)
                .Where(b => b.Personalinfoid == profile.PersonalinfoId)
                .ToListAsync();

            var model = new AccountViewModel
            {
                Profile = profile,
                Bookings = bookings
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new Personalinfo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Lastname,DOB,Email,PhoneNumber")] Personalinfo personalinfo)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            if (await _context.Personalinfos.AnyAsync(p => p.IdentityUserId == userId))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                personalinfo.IdentityUserId = userId;
                _context.Add(personalinfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(personalinfo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null || id != profile.PersonalinfoId)
            {
                return NotFound();
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonalinfoId,IdentityUserId,Name,Lastname,DOB,Email,PhoneNumber")] Personalinfo personalinfo)
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null || id != personalinfo.PersonalinfoId || profile.IdentityUserId != personalinfo.IdentityUserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(personalinfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(personalinfo);
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
