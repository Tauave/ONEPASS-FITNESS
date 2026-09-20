
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Data;
using System.Security.Claims;

public class ProgressController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProgressController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PROGRESS
    public async Task<IActionResult> Index()    
    {
        if (!User.Identity?.IsAuthenticated ?? true) return Challenge();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var items = await _context.Progress
            .Where(p => p.AppUserId == userId)
            .ToListAsync();
        return View(items);
    }

    // GET: PROGRESSS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        if (!User.Identity?.IsAuthenticated ?? true) return Challenge();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var progress = await _context.Progress
            .FirstOrDefaultAsync(m => m.ProgressId == id && m.AppUserId == userId);
        if (progress == null)
        {
            return NotFound();
        }

        return View(progress);
    }

    // GET: PROGRESS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PROGRESSS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProgressId,Weight")] Progress progress)
    {
        if (!User.Identity?.IsAuthenticated ?? true) return Challenge();

        // set owner and timestamp server-side so the form cannot override them
        progress.AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        progress.DateRecorded = DateOnly.FromDateTime(DateTime.UtcNow);

        if (ModelState.IsValid)
        {
            _context.Add(progress);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(progress);
    }

    // GET: PROGRESS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var progress = await _context.Progress.FindAsync(id);
        if (progress == null)
        {
            return NotFound();
        }
        return View(progress);
    }

    // POST: PROGRESS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("ProgressId,Weight,DateRecorded,appUser")] Progress progress)
    {
        if (id != progress.ProgressId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(progress);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgressExists(progress.ProgressId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(progress);
    }

    // GET: PROGRESS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        if (!User.Identity?.IsAuthenticated ?? true) return Challenge();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var progress = await _context.Progress
            .FirstOrDefaultAsync(m => m.ProgressId == id && m.AppUserId == userId);
        if (progress == null)
        {
            return NotFound();
        }

        return View(progress);
    }

    // POST: PROGRESS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!User.Identity?.IsAuthenticated ?? true) return Challenge();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var progress = await _context.Progress
            .FirstOrDefaultAsync(p => p.ProgressId == id && p.AppUserId == userId);

        if (progress == null)
        {
            return NotFound();
        }

        _context.Progress.Remove(progress);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProgressExists(int id)
    {
        return _context.Progress.Any(e => e.ProgressId == id);
    }
}
