using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectRAS.Models
{
    public class Result
    {
        [Key, ForeignKey("User")]
        public Guid User_id { get; set; }
        public int? Score { get; set; }
        public virtual User? User { get; set; }
    }
}
