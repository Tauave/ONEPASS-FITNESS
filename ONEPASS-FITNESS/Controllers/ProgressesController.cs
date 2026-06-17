
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Data;

public class ProgressesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProgressesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PROGRESSS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Progress.ToListAsync());
    }

    // GET: PROGRESSS/Details/5
    public async Task<IActionResult> Details(int? progressid)
    {
        if (progressid == null)
        {
            return NotFound();
        }

        var progress = await _context.Progress
            .FirstOrDefaultAsync(m => m.ProgressId == progressid);
        if (progress == null)
        {
            return NotFound();
        }

        return View(progress);
    }

    // GET: PROGRESSS/Create
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

    // GET: PROGRESSS/Edit/5
    public async Task<IActionResult> Edit(int? progressid)
    {
        if (progressid == null)
        {
            return NotFound();
        }

        var progress = await _context.Progress.FindAsync(progressid);
        if (progress == null)
        {
            return NotFound();
        }
        return View(progress);
    }

    // POST: PROGRESSS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? progressid, [Bind("ProgressId,Weight,DateRecorded,appUser")] Progress progress)
    {
        if (progressid != progress.ProgressId)
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

    // GET: PROGRESSS/Delete/5
    public async Task<IActionResult> Delete(int? progressid)
    {
        if (progressid == null)
        {
            return NotFound();
        }

        var progress = await _context.Progress
            .FirstOrDefaultAsync(m => m.ProgressId == progressid);
        if (progress == null)
        {
            return NotFound();
        }

        return View(progress);
    }

    // POST: PROGRESSS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? progressid)
    {
        var progress = await _context.Progress.FindAsync(progressid);
        if (progress != null)
        {
            _context.Progress.Remove(progress);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProgressExists(int? progressid)
    {
        return _context.Progress.Any(e => e.ProgressId == progressid);
    }
}
