using BlogIt.Data;
using BlogIt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BlogIt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        
        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()
        {
            // To display all posts in home index page
            var blogs = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.User).OrderByDescending(b => b.Date).ToList();

            // To store userProfiles of that blog id, to display userName and user profile pic
            var userProfileDict = new Dictionary<string, UserProfie>();

            // To store Comments of that blog id 
            var CommentsDict = new Dictionary<string, List<Comment>>();
            foreach (var blog in blogs)
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == blog.UserId);
                userProfileDict[blog.UserId] = (UserProfie)userProfile;

                var comments = await _context.Comments.Where(c => c.BlogId == blog.Id).ToListAsync();

                CommentsDict[blog.Id] = comments;
                // To display userName and user profile pic  of all users who commented on the blog 
                foreach (var comment in comments)
                {
                    if(!userProfileDict.ContainsKey(comment.UserId))
                    {
                        userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == comment.UserId);
                        userProfileDict[blog.UserId] = (UserProfie)userProfile;
                    }
                }
            }

            var currentUserId = _userManager.GetUserId(this.User);

            //  Storing following list and current userId to Not display follow button on those values
            ViewBag.thisUserId = currentUserId;
            ViewBag.FollowingList =  _context.Followers.Include(f => f.User).Where(f => f.FollowerId == currentUserId).Select(f => f.UserId).ToList();
            ViewBag.UserProfiles = userProfileDict;
            ViewBag.Comments = CommentsDict;    
            return View(blogs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}