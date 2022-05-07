using RopeysDVD.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class Loan
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int LoanNumber { get; set; }
        public int LoanTypeNumber { get; set; }
        public int CopyNumber { get; set; }
        public int MemberNumber { get; set; }
        [Required]
        public DateTime DateOut { get; set; }
        [Required]
        public DateTime DateDue { get; set; }
        public DateTime ReturnedDate { get; set; }
        public string Status { get; set; }
        [ForeignKey("LoanTypeNumber")]
        public LoanType LoanType { get; set; }
        [ForeignKey("CopyNumber")]
        public DVDCopy DVDCopy { get; set; }
        [ForeignKey("MemberNumber")]
        public Member Member { get; set; }
    }
}
