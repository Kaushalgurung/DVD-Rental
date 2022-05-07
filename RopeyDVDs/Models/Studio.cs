using System.ComponentModel.DataAnnotations;

namespace RopeysDVD.Models
{
    public class Studio
    {
        [Key]
        public int StudioNumber { get; set; }
        [Required]
        public string StudioName { get; set; }
    }
}
