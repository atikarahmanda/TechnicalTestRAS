using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectRAS.Models
{
    public class Request
    {
        [Key, ForeignKey("User")]
        public Guid User_id { get; set; }
        public bool? Status_test { get; set; }
        public virtual User? User { get; set; }
    }
}
