using RopeysDVD.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class CastMember
    {
        [Key]
        public int Id { get; set; }
        public int ActorNumber { get; set; }
        public int DVDNumber { get; set; }
        [ForeignKey("DVDNumber")]
        public DVDTitle DVDTitle { get; set; }
        [Key]
        [ForeignKey("ActorNumber")]
        public Actor Actor { get; set; }
    }
}
