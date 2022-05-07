using System.ComponentModel.DataAnnotations;

namespace RopeysDVD.Models
{
    public class LoanType
    {
        [Key]
        public int LoanTypeNumber { get; set; }
        [Required]
        public string LoanTypeName { get; set; }
        [Required]
        public string LoanDuration { get; set; }

    }
}
