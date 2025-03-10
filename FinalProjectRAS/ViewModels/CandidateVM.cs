namespace FinalProjectRAS.ViewModels
{
    public class CandidateVM
    {
        public Guid User_id { get; set; }
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public DateTime? Deadline { get; set; }
        public bool? StatusTest { get; set; }

    }
}
