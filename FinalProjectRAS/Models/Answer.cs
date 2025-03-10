using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectRAS.Models
{
    public class Answer
    {
        [Key]
        public string? Answer_id { get; set; } = Guid.NewGuid().ToString();
        public virtual User? User { get; set; }
        [ForeignKey("User")]
        public Guid User_id { get; set; }
        public virtual Question? Question { get; set; }
        [ForeignKey("Question")]
        public string? Question_id { get; set; }
        public virtual Option? Option { get; set; }
        [ForeignKey("Option")]
        public string? Option_id { get; set; }
        public int? Point { get; set; }
    }
}