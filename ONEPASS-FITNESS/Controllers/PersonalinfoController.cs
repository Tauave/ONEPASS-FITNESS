
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Data;

public class PersonalinfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public PersonalinfoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PERSONALINFOS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Personalinfos.ToListAsync());
    }

    // GET: PERSONALINFOS/Details/5
    public async Task<IActionResult> Details(int? personalinfoid)
    {
        if (personalinfoid == null)
        {
            return NotFound();
        }

        var personalinfo = await _context.Personalinfos
            .FirstOrDefaultAsync(m => m.PersonalinfoId == personalinfoid);
        if (personalinfo == null)
        {
            return NotFound();
        }

        return View(personalinfo);
    }

    // GET: PERSONALINFOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PERSONALINFOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("PersonalinfoId,IdentityUserId,Name,Lastname,DOB,Email,PhoneNumber,ClassBookings,Progress")] Personalinfo personalinfo)
    {
        if (ModelState.IsValid)
        {
            _context.Add(personalinfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(personalinfo);
    }

    // GET: PERSONALINFOS/Edit/5
    public async Task<IActionResult> Edit(int? personalinfoid)
    {
        if (personalinfoid == null)
        {
            return NotFound();
        }

        var personalinfo = await _context.Personalinfos.FindAsync(personalinfoid);
        if (personalinfo == null)
        {
            return NotFound();
        }
        return View(personalinfo);
    }

    // POST: PERSONALINFOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? personalinfoid, [Bind("PersonalinfoId,IdentityUserId,Name,Lastname,DOB,Email,PhoneNumber,ClassBookings,Progress")] Personalinfo personalinfo)
    {
        if (personalinfoid != personalinfo.PersonalinfoId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(personalinfo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonalinfoExists(personalinfo.PersonalinfoId))
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
        return View(personalinfo);
    }

    // GET: PERSONALINFOS/Delete/5
    public async Task<IActionResult> Delete(int? personalinfoid)
    {
        if (personalinfoid == null)
        {
            return NotFound();
        }

        var personalinfo = await _context.Personalinfos
            .FirstOrDefaultAsync(m => m.PersonalinfoId == personalinfoid);
        if (personalinfo == null)
        {
            return NotFound();
        }

        return View(personalinfo);
    }

    // POST: PERSONALINFOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? personalinfoid)
    {
        var personalinfo = await _context.Personalinfos.FindAsync(personalinfoid);
        if (personalinfo != null)
        {
            _context.Personalinfos.Remove(personalinfo);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PersonalinfoExists(int? personalinfoid)
    {
        return _context.Personalinfos.Any(e => e.PersonalinfoId == personalinfoid);
    }
}
