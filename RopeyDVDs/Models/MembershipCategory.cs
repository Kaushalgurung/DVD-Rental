using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class MembershipCategory
    {
        public MembershipCategory()
            {
                Member =new List<Member>();
            }

        [Key]
        public int MembershipCategoryNumber { get; set; }
        [Required]
        public string MembershipCategoryDescription { get; set; }
        [Required]
        public int TotalLoans { get; set; }

        public List<Member> Member { get; set; }
    }
}
