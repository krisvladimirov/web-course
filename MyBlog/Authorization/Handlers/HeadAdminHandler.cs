using Microsoft.AspNetCore.Identity;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.Data.Seeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace MyBlog.Authorization.Handlers
{
    public class HeadAdminHandler : AuthorizationHandler<OperationAuthorizationRequirement, Post>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HeadAdminHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                        OperationAuthorizationRequirement requirement,
                                                        Post resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // The admin can be perform any operation on the posts
            if (context.User.IsInRole(Constants.HeadAdmin))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
