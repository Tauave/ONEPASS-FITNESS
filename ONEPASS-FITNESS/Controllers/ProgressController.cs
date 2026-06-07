using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Controllers
{
    [Authorize]
    public class ProgressController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProgressController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
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

            var entries = await _context.Progress
                .Where(p => p.Personalinfoid == profile.PersonalinfoId)
                .OrderByDescending(p => p.DateRecorded)
                .ToListAsync();

            return View(entries);
        }

        public async Task<IActionResult> Create()
        {
            if (await GetCurrentProfileAsync() == null)
            {
                return RedirectToAction("Create", "Personalinfo");
            }

            return View(new Progress { DateRecorded = DateOnly.FromDateTime(DateTime.Today) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Weight,DateRecorded")] Progress progress)
        {
            var profile = await GetCurrentProfileAsync();
            if (profile == null)
            {
                return RedirectToAction("Create", "Personalinfo");
            }

            progress.Personalinfoid = profile.PersonalinfoId;

            if (ModelState.IsValid)
            {
                _context.Add(progress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(progress);
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
