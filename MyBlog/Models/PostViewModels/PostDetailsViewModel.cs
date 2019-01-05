using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models.PostViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }

        public List<Comment> Comments { get; set; }

        // For posting a comment
        public int PostId { get; set; }

        [Display(Name = "Type in your comment here")]
        public string CommentValue { get; set; }

        public ApplicationUser CommentOwner { get; set; }
    }
}
