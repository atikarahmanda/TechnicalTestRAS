using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectRAS.Models
{
    public class Option
    {
        [Key]
        public string? Option_id { get; set; }

        [ForeignKey("Question")]
        public string? Question_id { get; set; }

        public string? Option_Text { get; set; }
        public string? Option_Image { get; set; }
        public bool? Is_Correct { get; set; }
        public virtual Question? Question { get; set; }
    }
}
