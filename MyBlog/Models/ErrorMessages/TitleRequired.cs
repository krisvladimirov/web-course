using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models.ErrorMessages
{
    public class TitleRequired : RequiredAttribute
    {
        public TitleRequired()
        {
            ErrorMessage = "Title required";
        }
    }
}
