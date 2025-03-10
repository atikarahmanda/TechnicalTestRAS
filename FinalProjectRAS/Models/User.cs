using FinalProjectRAS.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinalProjectRAS.Models
{
    public class User
    {
        [Key]
        public Guid User_id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Fullname { get; set; }
        public Role? Role { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Deadline { get; set; }

        [JsonIgnore]
        public virtual Request? Request { get; set; }

        [JsonIgnore]
        public virtual Result? Result { get; set; }
        public virtual ICollection<Answer>? Answers { get; set; }
    }
}
