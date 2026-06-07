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
    public class ProgressController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgressController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Progress
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Progress.Include(p => p.Personalinfo);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Progress/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var progress = await _context.Progress
                .Include(p => p.Personalinfo)
                .FirstOrDefaultAsync(m => m.ProgressId == id);
            if (progress == null)
            {
                return NotFound();
            }

            return View(progress);
        }

        // GET: Progress/Create
        public IActionResult Create()
        {
            ViewData["Personalinfoid"] = new SelectList(_context.Personalinfos, "PersonalinfoId", "Email");
            return View();
        }

        // POST: Progress/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgressId,Personalinfoid,Weight,DateRecorded")] Progress progress)
        {
            if (ModelState.IsValid)
            {
                _context.Add(progress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Personalinfoid"] = new SelectList(_context.Personalinfos, "PersonalinfoId", "Email", progress.Personalinfoid);
            return View(progress);
        }

        // GET: Progress/Edit/5
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
            ViewData["Personalinfoid"] = new SelectList(_context.Personalinfos, "PersonalinfoId", "Email", progress.Personalinfoid);
            return View(progress);
        }

        // POST: Progress/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProgressId,Personalinfoid,Weight,DateRecorded")] Progress progress)
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
            ViewData["Personalinfoid"] = new SelectList(_context.Personalinfos, "PersonalinfoId", "Email", progress.Personalinfoid);
            return View(progress);
        }

        // GET: Progress/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var progress = await _context.Progress
                .Include(p => p.Personalinfo)
                .FirstOrDefaultAsync(m => m.ProgressId == id);
            if (progress == null)
            {
                return NotFound();
            }

            return View(progress);
        }

        // POST: Progress/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var progress = await _context.Progress.FindAsync(id);
            if (progress != null)
            {
                _context.Progress.Remove(progress);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProgressExists(int id)
        {
            return _context.Progress.Any(e => e.ProgressId == id);
        }
    }
}
