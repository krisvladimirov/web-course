using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class Comment
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Comment")]
        [StringLength(500)]
        public string CommentValue { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CreationDate { get; set; }
        
        public virtual ApplicationUser Owner { get; set; }
       
        public virtual Post BelongingPost { get; set; }
    }
}
