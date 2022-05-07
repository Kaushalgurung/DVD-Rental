using System.ComponentModel.DataAnnotations;

namespace RopeysDVD.Models
{
    public class DVDCategory
    {
        [Key]
            public int CategoryNumber { get; set; }
            [Required]
            public string CategoryDescription { get; set; }
            [Required]
            public Boolean AgeRestricted { get; set; }
    }


}
