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
    public class CastMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CastMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Authentication
        [Authorize(Roles = "Manager, Assistant")]

        // GET: CastMembers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CastMember.Include(c => c.Actor).Include(c => c.DVDTitle);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET CastMembers Details
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var castMember = await _context.CastMember
                .Include(c => c.Actor)
                .Include(c => c.DVDTitle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (castMember == null)
            {
                return NotFound();
            }

            return View(castMember);
        }

        // GET CastMembers Create View
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult Create()
        {
            ViewData["ActorNumber"] = new SelectList(_context.Actors, "ActorNumber", "ActorFirstName");
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitle, "DVDNumber", "TitleName");
            return View();
        }

        // POST CastMembers Details into Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int DVDNumber, int ActorNumber, CastMember castMember)
        {
            castMember.DVDNumber = DVDNumber;
            castMember.ActorNumber = ActorNumber;
            try
            {
                _context.Add(castMember);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return null;
            }
        }


        // GET CastMembers Edit View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var castMember = await _context.CastMember.FindAsync(id);
            if (castMember == null)
            {
                return NotFound();
            }
            ViewData["ActorNumber"] = new SelectList(_context.Actors, "ActorNumber", "ActorFirstName", castMember.ActorNumber);
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitle, "DVDNumber", "TitleName", castMember.DVDNumber);
            return View(castMember);
        }

        // Update CastMembers Details
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int DVDNumber, int ActorNumber, CastMember castMember)
        {
            castMember.DVDNumber = DVDNumber;
            castMember.ActorNumber = ActorNumber;
            try
            {
                _context.Add(castMember);
                _context.SaveChanges();
                return RedirectToAction("Index"); 
            }
            catch (Exception)
            {
                return null;
            }
        }


        // GET CastMembers Delete View
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var castMember = await _context.CastMember
                .Include(c => c.Actor)
                .Include(c => c.DVDTitle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (castMember == null)
            {
                return NotFound();
            }

            return View(castMember);
        }

        // Delete CastMembers Data
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var castMember = await _context.CastMember.FindAsync(id);
            _context.CastMember.Remove(castMember);
            await _context.SaveChangesAsync();
            //TempData["delete"] = "Cast Member Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool CastMemberExists(int id)
        {
            return _context.CastMember.Any(e => e.Id == id);
        }
    }
}
