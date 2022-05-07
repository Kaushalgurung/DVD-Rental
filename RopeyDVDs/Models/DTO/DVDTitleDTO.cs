namespace RopeyDVDs.Models.DTO
{
    public class DVDTitleDTO
    {
        public string Title { get; set; }
        public DateTime DateReleased { get; set; }
        public string Description { get; set; }
        public string ProducerName { get; set; }
        public string StudioName { get; set; }
        public Boolean RestrictedAge { get; set; }
    }
}
