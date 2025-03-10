using FinalProjectRAS.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectRAS.Models
{
    public class Question
    {
        [Key]
        public string? Question_id { get; set; }
        public Category? Category { get; set; }
        public Level? Level { get; set; }
        public string? Question_text { get; set; }
        public string? ImagePath { get; set; }
        public virtual ICollection<Option>? Options { get; set; }

    }

}
