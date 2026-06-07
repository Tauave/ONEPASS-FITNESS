using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;

namespace ONEPASS_FITNESS.Controllers
{
    public class ClassBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassBookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClassBookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ClassBookings.Include(c => c.Class).Include(c => c.Personalinfo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ClassBookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classBookings = await _context.ClassBookings
                .Include(c => c.Class)
                .Include(c => c.Personalinfo)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (classBookings == null)
            {
                return NotFound();
            }

            return View(classBookings);
        }

        // GET: ClassBookings/Create
        public IActionResult Create()
        {
            ViewData["Classid"] = new SelectList(_context.Classes, "Classid", "Classname");
            ViewData["Personalinfoid"] = new SelectList(_context.Personalinfos, "PersonalinfoId", "Email");
            return View();
        }

        // POST: ClassBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,Classid,Personalinfoid,BookingDate,AttendanceStatus")] ClassBookings classBookings)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classBookings);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Classid"] = new SelectList(_context.Classes, "Classid", "Classname", classBookings.Classid);
            ViewData["Personalinfoid"] = new SelectList(_context.Personalinfos, "PersonalinfoId", "Email", classBookings.Personalinfoid);
            return View(classBookings);
        }

        // GET: ClassBookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classBookings = await _context.ClassBookings.FindAsync(id);
            if (classBookings == null)
            {
                return NotFound();
            }
            ViewData["Classid"] = new SelectList(_context.Classes, "Classid", "Classname", classBookings.Classid);
            ViewData["Personalinfoid"] = new SelectList(_context.Personalinfos, "PersonalinfoId", "Email", classBookings.Personalinfoid);
            return View(classBookings);
        }

        // POST: ClassBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,Classid,Personalinfoid,BookingDate,AttendanceStatus")] ClassBookings classBookings)
        {
            if (id != classBookings.BookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classBookings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassBookingsExists(classBookings.BookingID))
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
            ViewData["Classid"] = new SelectList(_context.Classes, "Classid", "Classname", classBookings.Classid);
            ViewData["Personalinfoid"] = new SelectList(_context.Personalinfos, "PersonalinfoId", "Email", classBookings.Personalinfoid);
            return View(classBookings);
        }

        // GET: ClassBookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classBookings = await _context.ClassBookings
                .Include(c => c.Class)
                .Include(c => c.Personalinfo)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (classBookings == null)
            {
                return NotFound();
            }

            return View(classBookings);
        }

        // POST: ClassBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classBookings = await _context.ClassBookings.FindAsync(id);
            if (classBookings != null)
            {
                _context.ClassBookings.Remove(classBookings);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassBookingsExists(int id)
        {
            return _context.ClassBookings.Any(e => e.BookingID == id);
        }
    }
}
