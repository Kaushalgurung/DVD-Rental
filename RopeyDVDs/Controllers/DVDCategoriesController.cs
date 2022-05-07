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
using RopeysDVD.Models;

namespace RopeyDVDs.Controllers
{
    public class DVDCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DVDCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET All DVD Categories
        //Authentication 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.DVDCategory.ToListAsync());
        }

        // GET selected DVDCategories Details
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCategory = await _context.DVDCategory
                .FirstOrDefaultAsync(m => m.CategoryNumber == id);
            if (dVDCategory == null)
            {
                return NotFound();
            }

            return View(dVDCategory);
        }

        // GET DVDCategories Create View
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult Create()
        {
            return View();
        }

        // POST DVDCategories Date in the Database 
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryNumber,CategoryDescription,AgeRestricted")] DVDCategory dVDCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dVDCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dVDCategory);
        }

        // GET DVDCategories Edit View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCategory = await _context.DVDCategory.FindAsync(id);
            if (dVDCategory == null)
            {
                return NotFound();
            }
            return View(dVDCategory);
        }

        // Update DVDCategories in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryNumber,CategoryDescription,AgeRestricted")] DVDCategory dVDCategory)
        {
            if (id != dVDCategory.CategoryNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dVDCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DVDCategoryExists(dVDCategory.CategoryNumber))
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
            return View(dVDCategory);
        }

        // GET DVDCategories Delete View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCategory = await _context.DVDCategory
                .FirstOrDefaultAsync(m => m.CategoryNumber == id);
            if (dVDCategory == null)
            {
                return NotFound();
            }

            return View(dVDCategory);
        }

        // Delete Cateogry Details from the database
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dVDCategory = await _context.DVDCategory.FindAsync(id);
            _context.DVDCategory.Remove(dVDCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DVDCategoryExists(int id)
        {
            return _context.DVDCategory.Any(e => e.CategoryNumber == id);
        }
    }
}
