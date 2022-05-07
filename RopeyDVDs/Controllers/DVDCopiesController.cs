#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.Data;
using RopeyDVDs.Enums;
using RopeyDVDs.Models;

namespace RopeyDVDs.Controllers
{
    public class DVDCopiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DVDCopiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET All DVDCopies Data
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DVDCopy.Include(d => d.DVDTitle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET selected  DVDCopies Details 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCopy = await _context.DVDCopy
                .Include(d => d.DVDTitle)
                .FirstOrDefaultAsync(m => m.CopyNumber == id);
            if (dVDCopy == null)
            {
                return NotFound();
            }

            return View(dVDCopy);
        }

        // GET: DVDCopies Create View
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult Create()
        {
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitle, "DVDNumber", "TitleName");
            return View();
        }

        // POST DVDCopies Dta into the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int CopyNumber, int DVDNumber, DateTime DatePurchased, DVDCopy dVDCopy)
        {
            dVDCopy.DVDNumber = DVDNumber;
            dVDCopy.CopyNumber = CopyNumber;
            dVDCopy.DatePurchased = DatePurchased;
            try
            {
                _context.Add(dVDCopy);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET DVDCopies Edit View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCopy = await _context.DVDCopy.FindAsync(id);
            if (dVDCopy == null)
            {
                return NotFound();
            }
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitle, "DVDNumber", "TitleName", dVDCopy.DVDNumber);
            return View(dVDCopy);
        }

        // Update DVD Copies Data in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int CopyNumber, int DVDNumber, DateTime DatePurchased, DVDCopy dVDCopy)
        {
            dVDCopy.DVDNumber = DVDNumber;
            dVDCopy.CopyNumber = CopyNumber;
            dVDCopy.DatePurchased = DatePurchased;
            try
            {
                _context.Add(dVDCopy);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET DVDCopies Delete View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCopy = await _context.DVDCopy
                .Include(d => d.DVDTitle)
                .FirstOrDefaultAsync(m => m.CopyNumber == id);
            if (dVDCopy == null)
            {
                return NotFound();
            }

            return View(dVDCopy);
        }

        // Delete DVDCopies fro the Database
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dVDCopy = await _context.DVDCopy.FindAsync(id);
            _context.DVDCopy.Remove(dVDCopy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DVDCopyExists(int id)
        {
            return _context.DVDCopy.Any(e => e.CopyNumber == id);
        }
    }
}
