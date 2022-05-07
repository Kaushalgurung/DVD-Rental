namespace RopeyDVDs.Models.DTO
{
    public class MemberWithNoLoanDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address { get; set; }

        public string DateOut { get; set; }

        public string DVDTitle { get; set; }

        public double NumberOfDays { get; set; }
    }
}
