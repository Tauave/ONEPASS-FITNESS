
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Models;
using ONEPASS_FITNESS.Data;

public class ClassBookingsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClassBookingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: CLASSBOOKINGSS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.ClassBookings.ToListAsync());
    }

    // GET: CLASSBOOKINGSS/Details/5
    public async Task<IActionResult> Details(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var classbookings = await _context.ClassBookings
            .FirstOrDefaultAsync(m => m.BookingID == bookingid);
        if (classbookings == null)
        {
            return NotFound();
        }

        return View(classbookings);
    }

    // GET: CLASSBOOKINGSS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CLASSBOOKINGSS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookingID,Classid,Class,Personalinfoid,BookingDate,AttendanceStatus,appUser")] ClassBookings classbookings)
    {
        if (ModelState.IsValid)
        {
            _context.Add(classbookings);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(classbookings);
    }

    // GET: CLASSBOOKINGSS/Edit/5
    public async Task<IActionResult> Edit(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var classbookings = await _context.ClassBookings.FindAsync(bookingid);
        if (classbookings == null)
        {
            return NotFound();
        }
        return View(classbookings);
    }

    // POST: CLASSBOOKINGSS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? bookingid, [Bind("BookingID,Classid,Class,Personalinfoid,BookingDate,AttendanceStatus,appUser")] ClassBookings classbookings)
    {
        if (bookingid != classbookings.BookingID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(classbookings);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassBookingsExists(classbookings.BookingID))
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
        return View(classbookings);
    }

    // GET: CLASSBOOKINGSS/Delete/5
    public async Task<IActionResult> Delete(int? bookingid)
    {
        if (bookingid == null)
        {
            return NotFound();
        }

        var classbookings = await _context.ClassBookings
            .FirstOrDefaultAsync(m => m.BookingID == bookingid);
        if (classbookings == null)
        {
            return NotFound();
        }

        return View(classbookings);
    }

    // POST: CLASSBOOKINGSS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? bookingid)
    {
        var classbookings = await _context.ClassBookings.FindAsync(bookingid);
        if (classbookings != null)
        {
            _context.ClassBookings.Remove(classbookings);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ClassBookingsExists(int? bookingid)
    {
        return _context.ClassBookings.Any(e => e.BookingID == bookingid);
    }
}
