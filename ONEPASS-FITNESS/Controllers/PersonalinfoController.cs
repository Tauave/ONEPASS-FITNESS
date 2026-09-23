using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Areas.Identity.Pages;
using Microsoft.AspNetCore.Authorization;

namespace ONEPASS_FITNESS.Controllers
{
    [Authorize]
    public class PersonalinfoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PersonalinfoController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Personalinfo
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var vm = new PersonalInfoViewModel
            {
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DOB = user.DOB.ToString("yyyy-MM-dd")
            };

            return View(vm);
        }

        // GET: /Personalinfo/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var vm = new PersonalInfoViewModel
            {
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DOB = user.DOB.ToString("yyyy-MM-dd")
            };

            return View(vm);
        }

        // POST: /Personalinfo/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PersonalInfoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            //Load user from the database not an ID from the form so people can't edit other users' info
            var user = await _userManager.GetUserAsync(User);
            //DOB is not updated here because it is not part of the form, and we don't want to allow users to change their DOB after account creation.
            if (user == null) return Challenge();

            user.Name = model.Name;
            user.Lastname = model.Lastname;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;


            //Only update the email if it has changed to avoid unnecessary database operations
            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var err in setEmailResult.Errors) ModelState.AddModelError(string.Empty, err.Description);
                    return View(model);
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors) ModelState.AddModelError(string.Empty, err.Description);
                return View(model);
            }

            TempData["StatusMessage"] = "Profile updated";
            return RedirectToAction(nameof(Index));
        }
    }
}
