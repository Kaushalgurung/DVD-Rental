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
using RopeysDVD.Models;

namespace RopeyDVDs.Controllers
{
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET All Loans Data
        //Authentication 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Loan.Include(l => l.DVDCopy).Include(l => l.LoanType).Include(l => l.Member);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET Selected Loans Details 
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan
                .Include(l => l.DVDCopy)
                .Include(l => l.LoanType)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.LoanNumber == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET Loans Create View
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult Create()
        {
            ViewData["CopyNumber"] = new SelectList(_context.DVDCopy, "CopyNumber", "CopyNumber");
            ViewData["LoanTypeNumber"] = new SelectList(_context.LoanType, "LoanTypeNumber", "LoanDuration");
            ViewData["MemberNumber"] = new SelectList(_context.Member, "MemberNumber", "MemberAddress");
            return View();
        }

        //Question No .6 (Validation fro the member wheter he/she is less than 18 for the DVD Loan.
        //Only the memebr older than 18 years can loan DVDs.
        //Also if the member has loan more than the membership category loan then the memebr can not loan DVDs.)
        // POST loan Detais in the Database
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int LoanTypeNumber, int CopyNumber, int MemberNumber, DateTime DateOut, DateTime DateDue, DateTime DateReturned, string Status, Loan loan)
        {
            loan.LoanTypeNumber = LoanTypeNumber;
            loan.CopyNumber = CopyNumber;
            loan.MemberNumber = MemberNumber;
            loan.DateOut = DateOut;
            loan.DateDue = DateDue;
            loan.ReturnedDate = DateReturned;
            loan.Status = Status;

            string loanTypeStr = HttpContext.Request.Form["LoanType.Loantype"];
            LoanType id = _context.LoanType.Where(l => l.LoanTypeName == loanTypeStr).FirstOrDefault();

            Member member = _context.Member.Where(l => l.MemberNumber == loan.MemberNumber).Include(m => m.MembershipCategory).FirstOrDefault();
            DVDCopy cat = _context.DVDCopy.Where(l => l.CopyNumber == loan.CopyNumber).Include(c => c.DVDTitle).ThenInclude(d => d.DVDCategory).FirstOrDefault();

            int remainingLoanCount = _context.Loan.Where(l => l.MemberNumber == loan.MemberNumber && l.ReturnedDate == null).Count();

            loan.DateOut = DateTime.Now;
            if (remainingLoanCount >= member.MembershipCategory.TotalLoans)
            {
                ModelState.AddModelError(string.Empty, "Member has too many DVD unreturned!");
                return View();
            }

            if (DateTime.Today.Year - member.MemberDateOfBirth.Year < 18 && cat.DVDTitle.DVDCategory.AgeRestricted)
            {
                ModelState.AddModelError(string.Empty, "Member is underaged for this DVD");
                return View();
            }
            try
            {
                _context.Add(loan);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET Loans Edit View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var loan = await _context.Loan.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            ViewData["CopyNumber"] = new SelectList(_context.DVDCopy, "CopyNumber", "CopyNumber", loan.CopyNumber);
            ViewData["LoanTypeNumber"] = new SelectList(_context.LoanType, "LoanTypeNumber", "LoanDuration", loan.LoanTypeNumber);
            ViewData["MemberNumber"] = new SelectList(_context.Member, "MemberNumber", "MemberAddress", loan.MemberNumber);
            return View(loan);
        }

        // Update Laon Details in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int LoanTypeNumber, int CopyNumber, int MemberNumber, DateTime DateOut, DateTime DateDue, DateTime ReturnedDate, string Status, Loan loan)
        {
            loan.LoanTypeNumber = LoanTypeNumber;
            loan.CopyNumber = CopyNumber;
            loan.MemberNumber = MemberNumber;
            loan.DateOut = DateOut;
            loan.DateDue = DateDue;
            loan.ReturnedDate = ReturnedDate;
            loan.Status = Status;
            try
            {
                _context.Update(loan);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET Loans Delete View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loan
                .Include(l => l.DVDCopy)
                .Include(l => l.LoanType)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.LoanNumber == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // Delete Selelcted Loan Details from the Database
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loan.FindAsync(id);
            _context.Loan.Remove(loan);
            await _context.SaveChangesAsync();
            TempData["delete"] = "Loan Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loan.Any(e => e.LoanNumber == id);
        }
    }
}
