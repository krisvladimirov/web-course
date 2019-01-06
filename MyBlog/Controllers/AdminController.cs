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
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser user = _userManager.FindByIdAsync(id);
            return View();
        }
    }
}
