using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminClassTypesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminClassTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /AdminClassTypes
        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;

            var items = await _db.ClassTypes
                .OrderBy(ct => ct.Name)
                .Select(ct => new ClassTypeListItemViewModel
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description,
                    DurationMinutes = ct.DurationMinutes,
                    IsActive = ct.IsActive,
                    UpcomingSessionCount = _db.ClassSessions.Count(s => s.ClassTypeId == ct.Id && s.StartTime > now),
                    TotalSessionCount = _db.ClassSessions.Count(s => s.ClassTypeId == ct.Id)
                })
                .ToListAsync();

            return View(items);
        }

        // GET: /AdminClassTypes/Create
        public IActionResult Create()
        {
            return View("Create", new ClassTypeViewModel());
        }

        // POST: /AdminClassTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassTypeViewModel model)
        {
            await ValidateNameIsUniqueAsync(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _db.ClassTypes.Add(new ClassType
            {
                Name = model.Name.Trim(),
                Description = model.Description?.Trim() ?? string.Empty,
                DurationMinutes = model.DurationMinutes,
                IsActive = true
            });

            await _db.SaveChangesAsync();
            TempData["Success"] = $"Class \"{model.Name.Trim()}\" was created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminClassTypes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var classType = await _db.ClassTypes.FindAsync(id);
            if (classType == null)
            {
                return NotFound();
            }

            return View(new ClassTypeViewModel
            {
                Id = classType.Id,
                Name = classType.Name,
                Description = classType.Description,
                DurationMinutes = classType.DurationMinutes
            });
        }

        // POST: /AdminClassTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassTypeViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var classType = await _db.ClassTypes.FindAsync(id);
            if (classType == null)
            {
                return NotFound();
            }

            await ValidateNameIsUniqueAsync(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            classType.Name = model.Name.Trim();
            classType.Description = model.Description?.Trim() ?? string.Empty;
            classType.DurationMinutes = model.DurationMinutes;

            await _db.SaveChangesAsync();
            TempData["Success"] = $"Class \"{classType.Name}\" was updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminClassTypes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var classType = await _db.ClassTypes.FindAsync(id);
            if (classType == null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;

            return View(new ClassTypeListItemViewModel
            {
                Id = classType.Id,
                Name = classType.Name,
                Description = classType.Description,
                DurationMinutes = classType.DurationMinutes,
                IsActive = classType.IsActive,
                UpcomingSessionCount = await _db.ClassSessions.CountAsync(s => s.ClassTypeId == id && s.StartTime > now),
                TotalSessionCount = await _db.ClassSessions.CountAsync(s => s.ClassTypeId == id)
            });
        }

        // POST: /AdminClassTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classType = await _db.ClassTypes.FindAsync(id);
            if (classType == null)
            {
                return NotFound();
            }

            //Stops classes from being deleted if they have sessions because it would break the database relationships
            //Instead the user is prompted to deactivate the class type.
            if (await _db.ClassSessions.AnyAsync(s => s.ClassTypeId == id))
            {
                TempData["Error"] = $"\"{classType.Name}\" has sessions, so it cannot be deleted. Deactivate it instead to hide it from new bookings.";
                return RedirectToAction(nameof(Index));
            }

            _db.ClassTypes.Remove(classType);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Class \"{classType.Name}\" was deleted.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminClassTypes/Toggle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Classes can be switched between active and inactive
        //Inactive classes are hidden from new bookings but existing sessions and bookings are unaffected.
        public async Task<IActionResult> Toggle(int id)
        {
            var classType = await _db.ClassTypes.FindAsync(id);
            if (classType == null)
            {
                return NotFound();
            }

            //Flip true to false or false to true
            classType.IsActive = !classType.IsActive;
            await _db.SaveChangesAsync();

            TempData["Success"] = classType.IsActive
                ? $"\"{classType.Name}\" is now active and bookable."
                : $"\"{classType.Name}\" is now inactive. Existing sessions and bookings are unchanged.";

            return RedirectToAction(nameof(Index));
        }

        //Adds an error if another class type with the same name already exists in the database (case-insensitive)
        //This is used in the Create and Edit actions to prevent duplicate class types the comparison ignores capitals so "Yoga" and "yoga" would be considered duplicates.
        private async Task ValidateNameIsUniqueAsync(ClassTypeViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return;
            }

            var name = model.Name.Trim();

            var duplicate = await _db.ClassTypes
                //ct.Id != model.Id makes sure that when editing an existing class type, it doesn't count itself as a duplicate
                .AnyAsync(ct => ct.Id != model.Id && ct.Name.ToLower() == name.ToLower());

            if (duplicate)
            {
                ModelState.AddModelError(nameof(model.Name), "A class with that name already exists.");
            }
        }
    }
}
