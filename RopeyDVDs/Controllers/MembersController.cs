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
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET All Members Data
        //Authentication
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Member.Include(m => m.MembershipCategory);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET Selected Members Details
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.MembershipCategory)
                .FirstOrDefaultAsync(m => m.MemberNumber == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET Members Create View
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult Create()
        {
            ViewData["MembershipCategoryNumber"] = new SelectList(_context.MembershipCategory, "MembershipCategoryNumber", "MembershipCategoryDescription");
            return View();
        }

        // POST Members Data in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int MemberNumber,string MemberFirstName,string MemberLastName,string MemberAddress,DateTime MemberDateOfBirth,int MembershipCategoryNumber, Member member)
        {
            member.MemberNumber = MemberNumber;
            member.MemberFirstName = MemberFirstName;
            member.MemberLastName = MemberLastName;
            member.MemberDateOfBirth = MemberDateOfBirth;
            try
            {
                _context.Update(member);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET Members Edit View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            ViewData["MembershipCategoryNumber"] = new SelectList(_context.MembershipCategory, "MembershipCategoryNumber", "MembershipCategoryDescription", member.MembershipCategoryNumber);
            return View(member);
        }

        // Update Member Data in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int MemberNumber, string MemberFirstName, string MemberLastName, string MemberAddress, DateTime MemberDateOfBirth, int MembershipCategoryNumber, Member member)
        {
            member.MemberNumber = MemberNumber;
            member.MemberFirstName = MemberFirstName;
            member.MemberLastName = MemberLastName;
            member.MemberDateOfBirth = MemberDateOfBirth;
            try
            {
                _context.Update(member);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET Members Delete View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.MembershipCategory)
                .FirstOrDefaultAsync(m => m.MemberNumber == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // Delete Member Data from the Database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Member.FindAsync(id);
            _context.Member.Remove(member);
            await _context.SaveChangesAsync();
            TempData["delete"] = "Member Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberNumber == id);
        }
    }
}
