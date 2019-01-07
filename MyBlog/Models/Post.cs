using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models
{
    public class Post
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        [StringLength(80)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(4000)]
        [Display(Name = "The post's content")]
        public string PostValue { get; set; }

        [Display(Name = "Posted on")]
        [ScaffoldColumn(false)]
        public DateTime CreationDate { get; set; }
        
        public virtual ApplicationUser Owner { get; set; }
    }
}
