using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using MyBlog.Data;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Authorization.Handlers
{
    public class PostAdminHandler : AuthorizationHandler<OperationAuthorizationRequirement, Post>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PostAdminHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                        OperationAuthorizationRequirement requirement,
                                                        Post resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (resource.Owner.Id == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
