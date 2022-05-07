#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.Data;
using RopeyDVDs.Models;
using RopeyDVDs.Models.DTO;
using RopeysDVD.Models;

namespace RopeyDVDs.Controllers
{
    public class DVDTitlesController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;

        public DVDTitlesController(ApplicationDbContext db)
        {
            applicationDbContext = db;
        }

        //Question No .1 (Searching the DVD Title according to the Actor LastName)
        //Authentication 
        [Authorize(Roles = "Manager, Assistant,User")]
        public IActionResult DVDTitleSearch(string surname)
        {
            var databaseContext = applicationDbContext.CastMember.Include(x => x.Actor).Include(x => x.DVDTitle);
            var actors = from a in databaseContext select a;
            if (!string.IsNullOrEmpty(surname))
            {
                actors = actors.Where(x => x.Actor.ActorSurname.Contains(surname));
            }

            return View(actors.ToList());
        }


        //Question No .2 (Allow the user to enter or select an actor’s name (Lastname) and see the titlesand the number of copies
        //on the shelves of all DVDs for which the Actor is aCastMember and which have at least one copy on the shelves.)
        [Authorize(Roles = "Manager, Assistant,User")]
        public IActionResult DVDOnSelves(string searchString)
        {
            var results = from dt in applicationDbContext.Set<DVDTitle>()
                          join cm in applicationDbContext.Set<CastMember>()
                              on dt.DVDNumber equals cm.DVDNumber
                          join dc in applicationDbContext.Set<DVDCopy>()
                                  .Where(c => applicationDbContext.Loan.Any(l => (c.CopyNumber == l.DVDCopy.CopyNumber && l.Status == "Returned")))
                              on dt.DVDNumber equals dc.DVDNumber
                          join a in applicationDbContext.Set<Actor>()
                                  .Where(x => x.ActorSurname.Contains(searchString))
                              on cm.ActorNumber equals a.ActorNumber
                          group new { dt, cm, dc } by new { dt.DVDNumber, dt.TitleName, a.ActorSurname }
            into grp
                          select new DVDOnSelevesDTO
                          {
                              DVDNumber = grp.Key.DVDNumber,
                              DVDCount = grp.Count(),
                              ActorSurname = grp.Key.ActorSurname,
                              Title = grp.Key.TitleName,
                          };
            ViewData["results"] = results;
            return View();
        }


        //Question No .3 (Searching the DVD according to the Member Name)
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> DVDSearchByMemberName(string dvdSearch)
        {
            var loanData = applicationDbContext.Member
                .Include(m => m.Loans) //From member models
                .ThenInclude(l => l.DVDCopy) //From loan models
            .Where(m => m.Loans.All(l => l.DateOut <= DateTime.UtcNow.AddDays(30)))
            .Where(m => m.MemberFirstName.Contains(dvdSearch)).FirstOrDefault();
            ViewData["member"] = loanData;
            if (loanData == null)
            {
                ViewData["loans"] = new List<Loan>();
            }
            else
            {
                ViewData["loans"] = loanData.Loans;
            }
            return View();

        }
        //Question No .4 (Showing all the details of the DVDs)
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult DVDDetails()
        {
            var AppDBContext = applicationDbContext.DVDTitle.Include(x => x.Producer).Include(x => x.Studio)
                .OrderBy(x => x.DateReleased).ToList();
            foreach (var data in AppDBContext)
            {
                List<string> actorList = applicationDbContext.CastMember
                    .Where(x => x.DVDNumber == data.DVDNumber)
                    .Include(x => x.Actor).OrderBy(x => x.Actor.ActorSurname)
                    .Select(x => x.Actor.ActorFirstName + " " + x.Actor.ActorSurname).ToList();
                string actors = string.Join(", ", actorList);
                data.actors = actors;
            }

            return View(AppDBContext);
        }

        //Question No .5 ( Searching the DVD Details  according to the Copy Number)
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult DVDDetailsSearchByCopyNumber(string copyNumber)
        {
            var ApplicationDbContext = applicationDbContext.Loan;

            if (!String.IsNullOrEmpty(copyNumber))
            {
                var loan = ApplicationDbContext.Where(l => l.CopyNumber == int.Parse(copyNumber)).Include(l => l.DVDCopy)
                    .ThenInclude(dc => dc.DVDTitle).Include(l => l.Member).FirstOrDefault();
                return View(loan);
            }
            return View();
        }

        //Question 7 (Total Number of Returned DVDs which are not on Loan)
        [Authorize(Roles = "Manager, Assistant")]
        public async Task<IActionResult> ReturnDVD(int id)
        {
            Loan loan = applicationDbContext.Loan.Where(x => x.CopyNumber == id).Include(x => x.DVDCopy).ThenInclude(x => x.DVDTitle).FirstOrDefault();
            if (loan != null)
            {
                if (loan.Status == "Loaned")
                {
                    loan.ReturnedDate = DateTime.Now;
                    applicationDbContext.Update(loan);
                    await applicationDbContext.SaveChangesAsync();
                    TimeSpan? days = loan.ReturnedDate - loan.DateDue;
                    if (days.Value.Days > 0)
                    {
                        ViewData["message"] = "DVD returned successfully and your total cost is " + days.Value.Days * loan.DVDCopy.DVDTitle.PenaltyCharge;
                        return View();
                    }
                    ViewData["message"] = "DVD returned successfully before due date";
                    return View();
                }
                else
                {
                    ViewData["message"] = "DVD already returned";
                    return View();
                }
            }

            ViewData["message"] = "DVD doesnot exist!!";
            return View();
        }

        //Question No .10 (Removing all the stocks of loaned DVDs which exceed more than 1 year)
        [Authorize(Roles = "Manager, Assistant")]
        public List<DVDCopy> DVDNotOnLoan()
        {
            List<DVDCopy> dvdCopies = applicationDbContext.DVDCopy.Include(x => x.DVDTitle).ToList();
            List<DVDCopy> newCopies = dvdCopies.Where(x => (DateTime.Now.Date - x.DatePurchased.Date).TotalDays >= 365).ToList();
            List<DVDCopy> dvdNotOnLoan = new List<DVDCopy>();
            foreach (var copy in newCopies)
            {
                List<Loan> copyLoans = applicationDbContext.Loan.Where(x => x.DVDCopy == copy && x.Status == "Loaned").ToList();
                if (copyLoans.Count == 0)
                {
                    dvdNotOnLoan.Add(copy);
                }
            }

            return dvdNotOnLoan;
        }
        // Function to show DVDs older than 365 Days
        public IActionResult DVDOlderThan365Days()
        {
            List<DVDCopy> dvdCopies = DVDNotOnLoan();
            return View(dvdCopies);
        }
        // Function to show detailed list of DVDs older than 365 Days
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult deleteDVDOlderThan365Days()
        {
            List<DVDCopy> dvdCopyNotOnLoan = DVDNotOnLoan();
            foreach (var dvdCopy in dvdCopyNotOnLoan)
            {
                try
                {
                    var copy_data = applicationDbContext.DVDCopy.Where(x => x.CopyNumber == dvdCopy.CopyNumber).First();
                    applicationDbContext.DVDCopy.Remove(copy_data);
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            ViewBag.DeleteMessage = "The Copy of DVD has been deleted successfully !!";
            return Redirect(null);
        }
        // Question No .11 (list of all DVD copies currently on loan)
        [Authorize(Roles = "Manager, Assistant")]
        [HttpGet]
        public IActionResult DVDCopiesOnLoan()
        {
            List<DVDCopiesOnLoanDTO> dvdCopiesOnLoanDtos = new List<DVDCopiesOnLoanDTO>();
            List<DVDTitle> dvdTitles = applicationDbContext.DVDTitle.ToList();
            List<DVDCopy> dvdCopies = new List<DVDCopy>();
            List<Loan> loans = new List<Loan>();
            Member member = new Member();
            foreach (var dvdTitle in dvdTitles)
            {
                dvdCopies = applicationDbContext.DVDCopy.Include(x => x.DVDTitle).Where(x => x.DVDTitle == dvdTitle).ToList();
                foreach (var dvdCopy in dvdCopies)
                {
                    loans = applicationDbContext.Loan.Include(x => x.DVDCopy).Include(x => x.Member).Where(x => x.DVDCopy == dvdCopy && x.Status == "Loaned").ToList();
                    if (loans != null)
                    {
                        foreach (var loan in loans)
                        {
                            member = applicationDbContext.Member.Where(x => x.MemberNumber == loan.Member.MemberNumber).First();
                            DVDCopiesOnLoanDTO copiesLoanDto = new DVDCopiesOnLoanDTO();
                            DateTime dateOut = loan.DateOut;
                            copiesLoanDto.DateOut = dateOut;
                            copiesLoanDto.Title = dvdTitle.TitleName;
                            copiesLoanDto.Name = member.MemberFirstName + "" + member.MemberLastName;
                            copiesLoanDto.CopyNumber = dvdCopy.CopyNumber;
                            dvdCopiesOnLoanDtos.Add(copiesLoanDto);
                        }
                    }
                }
            }
            dvdCopiesOnLoanDtos.OrderBy(x => x.DateOut).ThenBy(x => x.Title);
            return View(dvdCopiesOnLoanDtos);
        }

        //Question No .13 (List of all DVD titles in the shop where no copy of the title has been loaned in the last 31 days)
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult DVDWithNoLoanFor31Days()
        {
            List<DVDTitle> dvdTitles = applicationDbContext.DVDTitle.Include(x => x.Producer).Include(x => x.Studio).Include(x => x.DVDCategory).ToList();
            List<DVDTitleDTO> dVDTitleDTOs = new List<DVDTitleDTO>();
            List<Loan> copyLoans = new List<Loan>();
            List<Loan> copyLoansForLast31Days = new List<Loan>();
            bool count = false;
            foreach (var dvdTitle in dvdTitles)
            {
                var dvdCopies = applicationDbContext.DVDCopy.Include(x => x.DVDTitle).Where(x => x.DVDTitle == dvdTitle).ToList();
                foreach (DVDCopy copy in dvdCopies)
                {
                    copyLoans = applicationDbContext.Loan.Include(x => x.DVDCopy).Where(x => x.DVDCopy == copy && x.Status == "Loaned").ToList();
                    copyLoansForLast31Days.Where(x => (DateTime.Now.Date - x.DateOut.Date).TotalDays <= 31).ToList();
                    if (copyLoansForLast31Days.Count > 0)
                    {
                        count = true;
                        break;
                    }
                }

                if (count == false)
                {
                    DVDTitleDTO dVDTitle = new DVDTitleDTO();
                    dVDTitle.Title = dvdTitle.TitleName;
                    dVDTitle.Description = dvdTitle.DVDCategory.CategoryDescription;
                    dVDTitle.ProducerName = dvdTitle.Producer.ProducerName;
                    dVDTitle.DateReleased = dvdTitle.DateReleased;
                    dVDTitle.StudioName = dvdTitle.Studio.StudioName;
                    dVDTitle.RestrictedAge = dvdTitle.DVDCategory.AgeRestricted;
                    dVDTitleDTOs.Add(dVDTitle);
                }

                count = false;
            }

            return View(dVDTitleDTOs);
        }

        // GET: DVDTitles
        //public async Task<IActionResult> Index()
        //{
        //    var DbContext = applicationDbContext.DVDTitle.Include(d => d.DVDCategory).Include(d => d.Producer).Include(d => d.Studio);
        //    return View(await DbContext.ToListAsync());
        //}

        // GET: DVDTitles/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var dVDTitle = await applicationDbContext.DVDTitle
        //        .Include(d => d.DVDCategory)
        //        .Include(d => d.Producer)
        //        .Include(d => d.Studio)
        //        .FirstOrDefaultAsync(m => m.DVDNumber == id);
        //    if (dVDTitle == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(dVDTitle);
        //}

        // Question No .9 (Crud for DVDTitle)
        // GET DVDTitles Create View
        [Authorize(Roles = "Manager, Assistant")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["CategoryNumber"] = new SelectList(applicationDbContext.DVDCategory, "CategoryNumber", "CategoryDescription");
            ViewData["ProducerNumber"] = new SelectList(applicationDbContext.Producer, "ProducerNumber", "ProducerName");
            ViewData["StudioNumber"] = new SelectList(applicationDbContext.Studio, "StudioNumber", "StudioName");
            return View();
        }

        // POST DVDTitles Data in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int CategoryNumber, int StudioNumber, int ProducerNumber, string TitleName, DateTime DateReleased, Double StandardCharge, Double PenaltyCharge, DVDTitle dVDTitle)
        {
            dVDTitle.CategoryNumber = CategoryNumber;
            dVDTitle.ProducerNumber = ProducerNumber;
            dVDTitle.StudioNumber = StudioNumber;

            dVDTitle.TitleName = TitleName;
            dVDTitle.StandardCharge = StandardCharge;
            dVDTitle.PenaltyCharge = PenaltyCharge;
            dVDTitle.DateReleased = DateReleased;
            try
            {
                applicationDbContext.Add(dVDTitle);
                applicationDbContext.SaveChanges();
                return RedirectToAction("DVDDetails");
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET DVDTitles Edit View
        [Authorize(Roles = "Manager, Assistant")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var dVDTitle = applicationDbContext.DVDTitle.Find(id);
            if (dVDTitle == null)
            {
                return NotFound();
            }
            ViewData["CategoryNumber"] = new SelectList(applicationDbContext.DVDCategory, "CategoryNumber", "AgeRestricted", dVDTitle.CategoryNumber);
            ViewData["ProducerNumber"] = new SelectList(applicationDbContext.Producer, "ProducerNumber", "ProducerName", dVDTitle.ProducerNumber);
            ViewData["StudioNumber"] = new SelectList(applicationDbContext.Studio, "StudioNumber", "StudioName", dVDTitle.StudioNumber);
            return View(dVDTitle);
        }

        // POST DVDTitles in the Database
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int CategoryNumber, int StudioNumber, int ProducerNumber, string titleName, DateTime DateReleased, Double StandardCharge, Double PenaltyCharge, DVDTitle dVDTitle)
        {
            dVDTitle.CategoryNumber = CategoryNumber;
            dVDTitle.ProducerNumber = ProducerNumber;
            dVDTitle.StudioNumber = StudioNumber;
            dVDTitle.TitleName = titleName;
            dVDTitle.StandardCharge = StandardCharge;
            dVDTitle.PenaltyCharge = PenaltyCharge;
            dVDTitle.DateReleased = DateReleased;
            try
            {
                applicationDbContext.Update(dVDTitle);
                applicationDbContext.SaveChanges();
                return RedirectToAction("DVDDetails");
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET DVDTitles Delete View
        [Authorize(Roles = "Manager, Assistant")] 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await applicationDbContext.DVDTitle
                .Include(d => d.DVDCategory)
                .Include(d => d.Producer)
                .Include(d => d.Studio)
                .FirstOrDefaultAsync(m => m.DVDNumber == id);
            if (dVDTitle == null)
            {
                return NotFound();
            }

            return View(dVDTitle);
        }

        // Delete DVDDetais from the Database
        [Authorize(Roles = "Manager, Assistant")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dVDTitle = await applicationDbContext.DVDTitle.FindAsync(id);
            applicationDbContext.DVDTitle.Remove(dVDTitle);
            await applicationDbContext.SaveChangesAsync();
            TempData["delete"] = "Actor Deleted Successfully.";
            return RedirectToAction("DVDDetails");
        }

        private bool DVDTitleExists(int id)
        {
            return applicationDbContext.DVDTitle.Any(e => e.DVDNumber == id);
        }
    }
}
