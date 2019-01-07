using MyBlog.Models.ErrorMessages;
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

        [TitleRequired]
        [StringLength(100, ErrorMessage = "That is a pretty big title there, mind shortening it?")]
        [Display(Name = "Give it a title")]
        public string Title { get; set; }

        [ContentRequired]
        [StringLength(4000, MinimumLength = 100, ErrorMessage = "We all know you can do better, put at least 100 words")]
        [Display(Name = "The story")]
        public string PostValue { get; set; }

        [Display(Name = "Posted on")]
        [ScaffoldColumn(false)]
        public DateTime CreationDate { get; set; }
        
        public virtual ApplicationUser Owner { get; set; }
    }
}
