using System.ComponentModel.DataAnnotations;

namespace RopeysDVD.Models
{
    public class Producer
    {
        [Key]
        public int ProducerNumber { get; set; }
        [Required]
        public string ProducerName { get; set; }

    }
}
