using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.Areas.Identity.Data;
using RopeyDVDs.Models;
using RopeysDVD.Models;

namespace RopeyDVDs.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<DVDCategory> DVDCategory { get; set; }
        public DbSet<LoanType> LoanType { get; set; }
        public DbSet<MembershipCategory> MembershipCategory { get; set; }
        public DbSet<DVDTitle> DVDTitle { get; set; }
        public DbSet<Studio> Studio { get; set; }
        public DbSet<Producer> Producer { get; set; }
        public DbSet<DVDCopy> DVDCopy { get; set; }
        public DbSet<Loan> Loan { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<CastMember> CastMember { get; set; }
    }
}