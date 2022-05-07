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
using RopeyDVDs.Models;

namespace RopeyDVDs.Controllers
{
    public class MembershipCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembershipCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET All  MembershipCategories Data
        //Authentication 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.MembershipCategory.ToListAsync());
        }

        // GET Selected MembershipCategories Details 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipCategory = await _context.MembershipCategory
                .FirstOrDefaultAsync(m => m.MembershipCategoryNumber == id);
            if (membershipCategory == null)
            {
                return NotFound();
            }

            return View(membershipCategory);
        }

        // GET MembershipCategories Create View
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult Create()
        {
            return View();
        }

        // POST MembershipCategories in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MembershipCategoryNumber,MembershipCategoryDescription,TotalLoans")] MembershipCategory membershipCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershipCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(membershipCategory);
        }

        // GET MembershipCategories Edit View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipCategory = await _context.MembershipCategory.FindAsync(id);
            if (membershipCategory == null)
            {
                return NotFound();
            }
            return View(membershipCategory);
        }

        // Update MembershipCategories in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MembershipCategoryNumber,MembershipCategoryDescription,TotalLoans")] MembershipCategory membershipCategory)
        {
            if (id != membershipCategory.MembershipCategoryNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(membershipCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipCategoryExists(membershipCategory.MembershipCategoryNumber))
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
            return View(membershipCategory);
        }

        // GET MembershipCategories Delete View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membershipCategory = await _context.MembershipCategory
                .FirstOrDefaultAsync(m => m.MembershipCategoryNumber == id);
            if (membershipCategory == null)
            {
                return NotFound();
            }

            return View(membershipCategory);
        }

        // Delete MemebrshipCategory from the Database
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var membershipCategory = await _context.MembershipCategory.FindAsync(id);
            _context.MembershipCategory.Remove(membershipCategory);
            await _context.SaveChangesAsync();
            TempData["delete"] = "MembershipCategory Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipCategoryExists(int id)
        {
            return _context.MembershipCategory.Any(e => e.MembershipCategoryNumber == id);
        }
    }
}
