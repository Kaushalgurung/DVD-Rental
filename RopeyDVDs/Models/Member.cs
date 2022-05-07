using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class Member
    {
        public Member()
        {
            Loans = new HashSet<Loan>();
        }
        [Key]
        public int MemberNumber { get; set; }
        [Required]
        public string MemberFirstName { get; set; }
        [Required]
        public string MemberLastName { get; set; }
        [Required]
        public string MemberAddress { get; set; }
        public DateTime MemberDateOfBirth { get; set; }
        public int MembershipCategoryNumber { get; set; }
        [ForeignKey ("MembershipCategoryNumber")]
        public virtual MembershipCategory MembershipCategory { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }
        [NotMapped]
        public virtual int LoanCount { get; set; }
    }
}

