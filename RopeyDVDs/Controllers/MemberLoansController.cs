using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.Data;
using RopeyDVDs.Models;
using RopeyDVDs.Models.DTO;

namespace RopeyDVDs.Controllers
{
    public class MemberLoansController : Controller
    {
        private readonly ApplicationDbContext applicationDbContext;

        public MemberLoansController(ApplicationDbContext db)
        {
            applicationDbContext = db;
        }

        //Question No 8 (Showing all detils of the members who have taken loans)
        //Authentication
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult MemberLoanDetails()
        {
            List<MemberLoanDetailsDTO> memberLoanDetailsDtos = new List<MemberLoanDetailsDTO>();
            List<Member> memberList = applicationDbContext.Member.Include(x => x.MembershipCategory).ToList();
            if (memberList != null)
            {
                foreach (Member member in memberList)
                {
                    var membershipCategory = applicationDbContext.MembershipCategory.Where(x => x.MembershipCategoryNumber == member.MembershipCategory.MembershipCategoryNumber).First();
                    int totalLoan = applicationDbContext.Loan.Include(x => x.Member).Where(x => x.Member == member
                           && x.Status == "loaned").ToArray().Length;

                    if (totalLoan > 0)
                    {
                        MemberLoanDetailsDTO memberLoanDetail = new MemberLoanDetailsDTO();
                        memberLoanDetail.Address = member.MemberAddress;
                        memberLoanDetail.FirstName = member.MemberFirstName;
                        memberLoanDetail.LastName = member.MemberLastName;
                        memberLoanDetail.DateOfBirth = member.MemberDateOfBirth;
                        memberLoanDetail.TotalLoans = totalLoan;
                        memberLoanDetail.Description = membershipCategory.MembershipCategoryDescription;
                        memberLoanDetailsDtos.Add(memberLoanDetail);
                    }
                }

            }
            List<MemberLoanDetailsDTO> order = memberLoanDetailsDtos.OrderBy(x => x.FirstName).ToList();
            ViewBag.DTOS = order;
            return View(order);
        }

        //Question No .12 (List of all Members who have not borrowed any DVD in the last 31 days)
        [Authorize(Roles = "Manager, Assistant")]
        public IActionResult MemberWithNoLoanFor31Days ()
        {
            List<Member> members = new List<Member>();
            members = applicationDbContext.Member.ToList();
            List<MemberWithNoLoanDTO> memberWithNoLoanDtos = new List<MemberWithNoLoanDTO>();
            List<Loan> loans = new List<Loan>();
            DVDCopy dvdCopy = new DVDCopy();
            Loan loan = new Loan();
            string title = "";
            foreach (var member in members)
        {
                loans = applicationDbContext.Loan.Include(x => x.DVDCopy).Where(x => x.Member == member).ToList();
                var l = loans.Where(x => (DateTime.Now.Date - x.DateOut.Date).TotalDays > 31).ToList();
                foreach (var memberLoan in l)
                {
                    dvdCopy = applicationDbContext.DVDCopy.Include(x => x.DVDTitle).Where(x => x.CopyNumber == memberLoan.DVDCopy.CopyNumber).First();
                    loan = memberLoan;
                    var dvdtitles = applicationDbContext.DVDTitle.Where(x => x.DVDNumber == dvdCopy.DVDTitle.DVDNumber);
                    foreach (var dvdTitle in dvdtitles)
                    {
                        title = dvdTitle.TitleName;
                    }
                }

                if (l.Count > 0)
                {
                    MemberWithNoLoanDTO memberWithNoLoan = new MemberWithNoLoanDTO();
                    memberWithNoLoan.FirstName = member.MemberFirstName;
                    memberWithNoLoan.LastName = member.MemberLastName;
                    memberWithNoLoan.Address = member.MemberAddress;
                    memberWithNoLoan.DVDTitle = title;
                    memberWithNoLoan.DateOut = loan.DateOut.Date.ToLongDateString();
                    memberWithNoLoan.NumberOfDays = (DateTime.Now.Date - loan.DateOut).TotalDays;
                    memberWithNoLoanDtos.Add(memberWithNoLoan);
                }
            }

            return View(memberWithNoLoanDtos);
        }






















    }

}
