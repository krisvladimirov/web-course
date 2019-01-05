using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Seeds
{
    public class Constants
    {
        public const string Create = "Create";
        // Or in other words look up something
        public const string Read = "Read";
        // Or in other words edit something
        public const string Update = "Update";
        public const string Delete = "Delete";

        public const string HeadAdmin = "Administrator";
        public const string PostAdmin = "PostAdmin";
        public const string User = "User";
        public const string HeadAdminOrPostAdmin = HeadAdmin + "," + PostAdmin;
        public const string RegisteredUsers = HeadAdmin + "," + PostAdmin + "," + User;
    }
}
