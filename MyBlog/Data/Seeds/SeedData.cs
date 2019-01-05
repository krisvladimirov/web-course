﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Data.Seeds
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider service)
        {
            await CreateRoles(service);

            string adminId = await CreateUsers(service, "admin@email.com", "Password123!");
            await LinkUserToRole(service, adminId, Constants.HeadAdmin);

            string postAdmiOneId = await CreateUsers(service, "member1@email.com", "Password123!");
            await LinkUserToRole(service, postAdmiOneId, Constants.PostAdmin);

            string postAdminTwoId = await CreateUsers(service, "member2@email.com", "Password123!");
            await LinkUserToRole(service, postAdminTwoId, Constants.PostAdmin);


            //await CreateUsers(service);
            //RoleManager<IdentityRole> roleManager = service
            //    .GetRequiredService<RoleManager<IdentityRole>>();
            //await EnsureRolesAsync(roleManager);

            //UserManager<ApplicationUser> userManager = service.
            //    GetRequiredService<UserManager<ApplicationUser>>();
            //await EnsureTestAdminAsync(userManager);

        }

        private static async Task<string> CreateUsers(IServiceProvider service, string name, string password)
        {
            UserManager<ApplicationUser> userManager = service.GetRequiredService<UserManager<ApplicationUser>>();


            var user = await userManager.FindByNameAsync(name);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = name,
                    Email = name
                };
                await userManager.CreateAsync(user, password);
            }

            return user.Id;

        }

        private static async Task LinkUserToRole(IServiceProvider service, string userId, string role)
        {
            UserManager<ApplicationUser> userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<IdentityRole> roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            IdentityResult roleResult = null;

            var user = await userManager.FindByIdAsync(userId);

            roleResult = await userManager.AddToRoleAsync(user, role);

        }

        private static async Task CreateRoles(IServiceProvider service)
        {
            RoleManager<IdentityRole> roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            IdentityResult roleResult = null;
            string[] roles = { Constants.HeadAdmin, Constants.PostAdmin, Constants.User };

            foreach (var roleName in roles)
            {
                bool roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
