using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;
using System.Diagnostics;

namespace ONEPASS_FITNESS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var profile = await _context.Personalinfos
                    .FirstOrDefaultAsync(p => p.IdentityUserId == userId);
                if (profile != null)
                {
                    ViewBag.DisplayName = profile.Name;
                }
                else
                {
                    ViewBag.DisplayName = User.Identity.Name;
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
