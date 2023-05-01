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

        
        public IActionResult Index()
        {
            var blogs = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.User).OrderByDescending(b => b.Date).ToList();
            var userProfileDict = new Dictionary<string, UserProfie>();
            foreach (var blog in blogs)
            {
                var userProfile =  _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == blog.UserId);
                userProfileDict[blog.UserId] = (UserProfie)userProfile;
            }
            ViewBag.thisUserId = _userManager.GetUserId(this.User);

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