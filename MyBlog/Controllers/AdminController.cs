﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.Authorization.Operations;
using MyBlog.Data.Seeds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MyBlog.Models.AccountViewModels;

namespace MyBlog.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public AdminController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;

        }

        [HttpGet]
        [Authorize(Roles = Constants.HeadAdmin)]
        public async Task<IActionResult> Index()
        {
            var allUsers = _userManager.Users.ToList();

            UsersAndRolesViewModel UserAndRoleViewModel = new UsersAndRolesViewModel();

            List<AccountViewModel> AllUsersAndRoles = new List<AccountViewModel>();

            foreach (var usr in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(usr);
                AccountViewModel model = new AccountViewModel();
                if (roles[0] != Constants.HeadAdmin)
                {
                    model.UserId = usr.Id;
                    model.Email = usr.Email;
                    model.UserName = usr.UserName;
                    model.Roles = roles;
                    //var model = new AccountViewModel
                    //{
                    //    UserId = usr.Id,
                    //    Email = usr.Email,
                    //    UserName = usr.UserName,
                    //    Roles = await _userManager.GetRolesAsync(usr)
                    //};
                    AllUsersAndRoles.Add(model);
                }
            }

            UserAndRoleViewModel.UserAndRole = AllUsersAndRoles;

            return View(UserAndRoleViewModel);
        }

        [HttpGet]
        [Authorize(Roles = Constants.HeadAdmin)]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            AccountViewModel model = new AccountViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Constants.HeadAdmin)]
        public async Task<IActionResult> Edit(string id, [Bind("UserId, NewRole")] AccountViewModel model)
        {
            IdentityResult result = IdentityResult.Success;

            if (id != model.UserId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (model.NewRole == Constants.PostAdmin || model.NewRole == Constants.User)
                {
                    var user = await _userManager.FindByIdAsync(model.UserId);
                    var rolesForUser = await _userManager.GetRolesAsync(user);


                    if (user != null)
                    {
                        // Remove old role and any additional if present
                        foreach (var item in rolesForUser)
                        {
                            result = await _userManager.RemoveFromRoleAsync(user, item);
                            if (result != IdentityResult.Success)
                                break;
                        }

                        // Assign new role
                        await _userManager.AddToRoleAsync(user, model.NewRole);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        [Authorize(Roles = Constants.HeadAdmin)]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);

        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = Constants.HeadAdmin)]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var user = await _userManager.FindByIdAsync(id);
                var logins = await _userManager.GetLoginsAsync(user);
                var rolesForUser = await _userManager.GetRolesAsync(user);

                var userPosts =  _context.Post.Where(p => p.Owner == user).ToList();
                userPosts.ForEach(p => _context.Post.Remove(p));

                var userComments = _context.Comments.Where(c => c.Owner == user).ToList();
                userComments.ForEach(c => _context.Comments.Remove(c));

                using (var transaction = _context.Database.BeginTransaction())
                {
                    IdentityResult result = IdentityResult.Success;
                    foreach (var login in logins)
                    {
                        result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                        if (result != IdentityResult.Success)
                            break;
                    }
                    if (result == IdentityResult.Success)
                    {
                        foreach (var item in rolesForUser)
                        {
                            result = await _userManager.RemoveFromRoleAsync(user, item);
                            if (result != IdentityResult.Success)
                                break;
                        }
                    }
                    if (result == IdentityResult.Success)
                    {
                        result = await _userManager.DeleteAsync(user);
                        if (result == IdentityResult.Success)
                            transaction.Commit();
                    }
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }

        }
    }
}
