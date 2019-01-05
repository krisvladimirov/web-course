using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.Models.PostViewModels;
using MyBlog.Authorization.Operations;
using MyBlog.Data.Seeds;

namespace MyBlog.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public PostsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Roles = Constants.RegisteredUsers)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Post.Include(p => p.Owner).ToListAsync());
        }

        // GET: Posts/Details/5
        [HttpGet]
        [Authorize(Roles = Constants.RegisteredUsers)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post post = await _context.Post
                .Include(p => p.Owner)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            PostDetailsViewModel viewModel = await GetPostDetailsViewModel(post);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.RegisteredUsers)]
        public async Task<IActionResult> Details([Bind("PostId, CommentValue")] PostDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = await GetCurrentUserAsync();
                Comment comment = new Comment
                {
                    CommentValue = viewModel.CommentValue,
                    Owner = user,
                    CreationDate = DateTime.Now
                    //String.format"{0:g}"

                };

                Post post = await _context.Post
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(m => m.Id == viewModel.PostId);
                if (post == null)
                {
                    return NotFound();
                }
                comment.BelongingPost = post;
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                viewModel = await GetPostDetailsViewModel(post);

            }

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Roles = Constants.HeadAdminOrPostAdmin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.HeadAdminOrPostAdmin)]
        public async Task<IActionResult> Create([Bind("PostValue,Title")] Post post)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await GetCurrentUserAsync();
                post.Owner = user;
                post.CreationDate = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [HttpGet]
        [Authorize(Roles = Constants.HeadAdminOrPostAdmin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post post = await _context.Post.SingleOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            AuthorizationResult isAuthorized = await _authorizationService.AuthorizeAsync(User, post, Operations.Update);
            if (!isAuthorized.Succeeded)
            {
                return RedirectToAction("Index");
            }


            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.HeadAdminOrPostAdmin)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostValue,Title")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [HttpGet]
        [Authorize(Roles = Constants.HeadAdminOrPostAdmin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post post = await _context.Post
                .SingleOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            AuthorizationResult isAuthorized = await _authorizationService.AuthorizeAsync(User, post, Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return RedirectToAction("Index");
            }

            return View(post);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.HeadAdminOrPostAdmin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Post post = await _context.Post.SingleOrDefaultAsync(m => m.Id == id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a post exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        private async Task<PostDetailsViewModel> GetPostDetailsViewModel(Post post)
        {
            PostDetailsViewModel viewModel = new PostDetailsViewModel();

            viewModel.Post = post;

            List<Comment> comments = await _context.Comments
                .Where(m => m.BelongingPost == post).ToListAsync();

            viewModel.Comments = comments;

            return viewModel;
        }

        /// <summary>
        /// Retrieves the currently logged user
        /// </summary>
        /// <returns></returns>
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

    }

}
