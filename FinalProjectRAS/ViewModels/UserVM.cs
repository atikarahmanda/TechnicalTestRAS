using FinalProjectRAS.Utils;

namespace FinalProjectRAS.ViewModels
{
    public class UserVM
    {
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? Role { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? Deadline { get; set; }
    }
}
