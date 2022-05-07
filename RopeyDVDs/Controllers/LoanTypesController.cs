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
    public class LoanTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoanTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET:  All LoanTypes Data
        //Authentication 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.LoanType.ToListAsync());
        }

        // Show Selected LoanTypes Details 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanType = await _context.LoanType
                .FirstOrDefaultAsync(m => m.LoanTypeNumber == id);
            if (loanType == null)
            {
                return NotFound();
            }

            return View(loanType);
        }

        // GET LoanTypes Create View
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult Create()
        {
            return View();
        }

        // POST LoanTypes Details in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoanTypeNumber,LoanTypeName,LoanDuration")] LoanType loanType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loanType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loanType);
        }

        // GET LoanTypes Edit View 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanType = await _context.LoanType.FindAsync(id);
            if (loanType == null)
            {
                return NotFound();
            }
            return View(loanType);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST LoanTypes in the Database
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanTypeNumber,LoanTypeName,LoanDuration")] LoanType loanType)
        {
            if (id != loanType.LoanTypeNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loanType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanTypeExists(loanType.LoanTypeNumber))
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
            return View(loanType);
        }

        // GET LoanTypes Delete View 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanType = await _context.LoanType
                .FirstOrDefaultAsync(m => m.LoanTypeNumber == id);
            if (loanType == null)
            {
                return NotFound();
            }

            return View(loanType);
        }

        // Delete LoanTypes from the Database
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loanType = await _context.LoanType.FindAsync(id);
            _context.LoanType.Remove(loanType);
            await _context.SaveChangesAsync();
            TempData["delete"] = "Loan Type Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool LoanTypeExists(int id)
        {
            return _context.LoanType.Any(e => e.LoanTypeNumber == id);
        }
    }
}
