using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RopeysDVD.Models
{
    public class Actor
    {
        [Key]
        public int ActorNumber { get; set; }
        [Required(ErrorMessage = "Actor FirstName is Required.")]
        public string ActorFirstName { get; set; }
        [Required(ErrorMessage = "Actor Surname is Required.")]
        public string ActorSurname { get; set; }

    }
}
