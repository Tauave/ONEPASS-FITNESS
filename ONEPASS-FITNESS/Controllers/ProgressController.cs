
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Data;

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
        return View(await _context.Progress.ToListAsync());
    }

    // GET: PROGRESSS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var progress = await _context.Progress
            .FirstOrDefaultAsync(m => m.ProgressId == id);
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
    public async Task<IActionResult> Create([Bind("ProgressId,Weight,DateRecorded,appUser")] Progress progress)
    {
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

        var progress = await _context.Progress
            .FirstOrDefaultAsync(m => m.ProgressId == id);
        if (progress == null)
        {
            return NotFound();
        }

        return View(progress);
    }

    // POST: PROGRESS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var progress = await _context.Progress.FindAsync(id);
        if (progress != null)
        {
            _context.Progress.Remove(progress);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProgressExists(int? id)
    {
        return _context.Progress.Any(e => e.ProgressId == id);
    }
}
