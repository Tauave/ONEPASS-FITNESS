
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Data;

public class ClassesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClassesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: CLASSESS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Classes.ToListAsync());
    }

    // GET: CLASSESS/Details/5
    public async Task<IActionResult> Details(int? classid)
    {
        if (classid == null)
        {
            return NotFound();
        }

        var classes = await _context.Classes
            .FirstOrDefaultAsync(m => m.Classid == classid);
        if (classes == null)
        {
            return NotFound();
        }

        return View(classes);
    }

    // GET: CLASSESS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CLASSESS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Classid,Classname,Date,Starttime,Endtime,Capacity,ClassBookings")] Classes classes)
    {
        if (ModelState.IsValid)
        {
            _context.Add(classes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(classes);
    }

    // GET: CLASSESS/Edit/5
    public async Task<IActionResult> Edit(int? classid)
    {
        if (classid == null)
        {
            return NotFound();
        }

        var classes = await _context.Classes.FindAsync(classid);
        if (classes == null)
        {
            return NotFound();
        }
        return View(classes);
    }

    // POST: CLASSESS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? classid, [Bind("Classid,Classname,Date,Starttime,Endtime,Capacity,ClassBookings")] Classes classes)
    {
        if (classid != classes.Classid)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(classes);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassesExists(classes.Classid))
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
        return View(classes);
    }

    // GET: CLASSESS/Delete/5
    public async Task<IActionResult> Delete(int? classid)
    {
        if (classid == null)
        {
            return NotFound();
        }

        var classes = await _context.Classes
            .FirstOrDefaultAsync(m => m.Classid == classid);
        if (classes == null)
        {
            return NotFound();
        }

        return View(classes);
    }

    // POST: CLASSESS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? classid)
    {
        var classes = await _context.Classes.FindAsync(classid);
        if (classes != null)
        {
            _context.Classes.Remove(classes);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ClassesExists(int? classid)
    {
        return _context.Classes.Any(e => e.Classid == classid);
    }
}
