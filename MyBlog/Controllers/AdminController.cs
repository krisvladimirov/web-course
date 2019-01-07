using Microsoft.AspNetCore.Mvc;
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
                var model = new AccountViewModel
                {
                    UserId = usr.Id,
                    Email = usr.Email,
                    UserName = usr.UserName,
                    Roles = await _userManager.GetRolesAsync(usr)
                };

                AllUsersAndRoles.Add(model);
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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        [Authorize(Roles = Constants.HeadAdmin)]
        public async Task<IActionResult> Edit(string id, [Bind("")] AccountViewModel model)
        {
            return View();
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
        [ValidateAntiForgeryToken]
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
                            transaction.Commit(); //only commit if user and all his logins/roles have been deleted  
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
