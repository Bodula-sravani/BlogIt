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
            var blogs = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.User).OrderByDescending(b => b.Date).ToList();
            var userProfileDict = new Dictionary<string, UserProfie>();
            foreach (var blog in blogs)
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == blog.UserId);
                userProfileDict[blog.UserId] = (UserProfie)userProfile;
            }
            var userId = _userManager.GetUserId(this.User);
            ViewBag.thisUserId = userId;
            ViewBag.FollowingList =  _context.Followers.Include(f => f.User).Where(f => f.FollowerId == userId).Select(f => f.UserId).ToList();
            ViewBag.UserProfiles = userProfileDict;
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