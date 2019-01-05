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
        public string Title { get; set; }

        [Required]
        [Display(Name = "The post's content")]
        public string PostValue { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CreationDate { get; set; }
        
        public virtual ApplicationUser Owner { get; set; }
    }
}
