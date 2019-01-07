using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Models.AccountViewModels
{
    public class AccountViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public IList<string> Roles { get; set; }

        public string NewRole { get; set; }

        public string Role { get; set; }
    }

    public class UsersAndRolesViewModel
    {
        public List<AccountViewModel> UserAndRole { get; set; }
    }
}
