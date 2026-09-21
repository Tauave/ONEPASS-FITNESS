using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;
using System.ComponentModel.DataAnnotations;

namespace ONEPASS_FITNESS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminClassTypesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AdminClassTypesController> _logger;

        public AdminClassTypesController(ApplicationDbContext db, ILogger<AdminClassTypesController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: AdminClassTypes
        public async Task<IActionResult> Index()
        {
            var classTypes = await _db.ClassTypes
                .AsNoTracking()
                .ToListAsync();

            var vm = new List<ClassTypeIndexViewModel>();
            foreach (var ct in classTypes)
            {
                var upcomingSessionCount = await _db.ClassSessions
                    .Where(s => s.ClassTypeId == ct.Id && s.StartTime > DateTime.UtcNow)
                    .CountAsync();

                vm.Add(new ClassTypeIndexViewModel
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description,
                    DurationMinutes = ct.DurationMinutes,
                    IsActive = ct.IsActive,
                    UpcomingSessionCount = upcomingSessionCount
                });
            }

            return View(vm);
        }

        // GET: AdminClassTypes/Create
        public IActionResult Create()
        {
            return View("Form", new ClassTypeFormViewModel());
        }

        // POST: AdminClassTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassTypeFormViewModel vm)
        {
            if (!ValidateClassTypeForm(vm))
            {
                return View("Form", vm);
            }

            var classType = new ClassType
            {
                Name = vm.Name!.Trim(),
                Description = vm.Description?.Trim(),
                DurationMinutes = vm.DurationMinutes,
                IsActive = true
            };

            _db.ClassTypes.Add(classType);
            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = "Class type created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class type");
                TempData["Error"] = "Error creating class type.";
                return View("Form", vm);
            }
        }

        // GET: AdminClassTypes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var classType = await _db.ClassTypes.FindAsync(id);
            if (classType == null)
                return NotFound();

            var vm = new ClassTypeFormViewModel
            {
                Id = classType.Id,
                Name = classType.Name,
                Description = classType.Description,
                DurationMinutes = classType.DurationMinutes
            };

            return View("Form", vm);
        }

        // POST: AdminClassTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClassTypeFormViewModel vm)
        {
            if (vm.Id != id)
                return BadRequest();

            if (!ValidateClassTypeForm(vm, excludeId: id))
            {
                return View("Form", vm);
            }

            var classType = await _db.ClassTypes.FindAsync(id);
            if (classType == null)
                return NotFound();

            classType.Name = vm.Name!.Trim();
            classType.Description = vm.Description?.Trim();
            classType.DurationMinutes = vm.DurationMinutes;

            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = "Class type updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class type");
                TempData["Error"] = "Error updating class type.";
                return View("Form", vm);
            }
        }

        // GET: AdminClassTypes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var classType = await _db.ClassTypes
                .Include(ct => ct.ClassSessions)
                .FirstOrDefaultAsync(ct => ct.Id == id);

            if (classType == null)
                return NotFound();

            if (classType.ClassSessions.Any())
            {
                TempData["Error"] = "Cannot delete a class type that has sessions. Please deactivate it instead.";
                return RedirectToAction(nameof(Index));
            }

            _db.ClassTypes.Remove(classType);
            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = "Class type deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class type");
                TempData["Error"] = "Error deleting class type.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: AdminClassTypes/Toggle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var classType = await _db.ClassTypes
                .Include(ct => ct.ClassSessions)
                .FirstOrDefaultAsync(ct => ct.Id == id);

            if (classType == null)
                return NotFound();

            // Warn if deactivating with upcoming sessions
            if (classType.IsActive && classType.ClassSessions.Any(s => s.StartTime > DateTime.UtcNow))
            {
                TempData["Warning"] = $"Deactivating this class type. It still has {classType.ClassSessions.Count(s => s.StartTime > DateTime.UtcNow)} upcoming session(s). Existing bookings will remain.";
            }

            classType.IsActive = !classType.IsActive;
            try
            {
                await _db.SaveChangesAsync();
                TempData["Success"] = classType.IsActive ? "Class type reactivated." : "Class type deactivated.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling class type");
                TempData["Error"] = "Error updating class type.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ValidateClassTypeForm(ClassTypeFormViewModel vm, int? excludeId = null)
        {
            var errors = new List<string>();

            // Name validation
            if (string.IsNullOrWhiteSpace(vm.Name))
            {
                errors.Add("Name is required.");
            }
            else if (vm.Name.Length > 50)
            {
                errors.Add("Name must not exceed 50 characters.");
            }
            else
            {
                // Check uniqueness (case-insensitive)
                var exists = _db.ClassTypes.AsNoTracking()
                    .Where(ct => ct.Name.ToLower() == vm.Name.ToLower())
                    .Where(ct => excludeId == null || ct.Id != excludeId)
                    .Any();

                if (exists)
                {
                    errors.Add("A class type with this name already exists.");
                }
            }

            // Duration validation
            if (vm.DurationMinutes < 15 || vm.DurationMinutes > 180)
            {
                errors.Add("Duration must be between 15 and 180 minutes.");
            }

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
                return false;
            }

            return true;
        }
    }

    public class ClassTypeFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name must not exceed 50 characters.")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Range(15, 180, ErrorMessage = "Duration must be between 15 and 180 minutes.")]
        public int DurationMinutes { get; set; } = 45;
    }

    public class ClassTypeIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; }
        public int UpcomingSessionCount { get; set; }
    }
}
