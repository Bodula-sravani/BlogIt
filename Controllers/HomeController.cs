﻿using BlogIt.Data;
using BlogIt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
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

        public async Task GetData(List<Blog> blogs, Dictionary<string, UserProfie> userProfileDict, Dictionary<string, List<Comment>> CommentsDict) 
        {
            // Gives userProfiles of each blog using blog id, to display userName and user profile pic
            // Gives Comments available on  each blog using blogid
            foreach (var blog in blogs)
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == blog.UserId);
                if (userProfile != null) userProfileDict[blog.UserId] = (UserProfie)userProfile;

                var comments = await _context.Comments.Where(c => c.BlogId == blog.Id).ToListAsync();

                CommentsDict[blog.Id] = comments;
                // To display userName and user profile pic of all users who commented on the blog 
                foreach (var comment in comments)
                {
                    if (!userProfileDict.ContainsKey(comment.UserId))
                    {
                        userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == comment.UserId);
                        if (userProfile != null) userProfileDict[comment.UserId] = (UserProfie)userProfile;
                    }
                }
            }
        }
        public async Task<IActionResult> Index()
        {
            // To display all posts in home index page
            var blogs = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.User).OrderByDescending(b => b.Date).ToList();

            // To store userProfiles of each blog using blog id, to display userName and user profile pic
            var userProfileDict = new Dictionary<string, UserProfie>();

            // To store Comments of each blog using blogid
            var CommentsDict = new Dictionary<string, List<Comment>>();

            await GetData(blogs,userProfileDict,CommentsDict);   

            var currentUserId = _userManager.GetUserId(this.User);
            //  Storing following list and current userId to Not display follow button on those values
            ViewBag.currentUserId = currentUserId;
            ViewBag.FollowingList =  _context.Followers.Include(f => f.User).Where(f => f.FollowerId == currentUserId).Select(f => f.UserId).ToList();
            ViewBag.UserProfiles = userProfileDict;
            ViewBag.Comments = CommentsDict;
            ViewData["CategoryNames"] = _context.BlogCategories.Select(c=>c.Name).ToList();
            return View(blogs);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Search(string searchString, string searchType)
        {
            var blogs = _context.Blogs.Include(b => b.BlogCategory).OrderByDescending(b => b.Date).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                if (searchType == "blogTitle")
                {
                    blogs = blogs.Where(b => b.Title.ToLower().Contains(searchString.ToLower())).ToList();
                }
                else if (searchType == "blogCategory")
                {
                    blogs = blogs.Where(b => b.BlogCategory.Name.ToLower().Contains(searchString.ToLower())).ToList();
                }
            }
            // To store userProfiles of each blog with blogid as key , to display userName and user profile pic
            var userProfileDict = new Dictionary<string, UserProfie>();

            // To store Comments of each blog with blog id as key
            var CommentsDict = new Dictionary<string, List<Comment>>();

            await GetData(blogs,userProfileDict, CommentsDict);

            var currentUserId = _userManager.GetUserId(this.User);
            //  Storing following list and current userId to Not display follow button on those values
            ViewBag.thisUserId = currentUserId;
            ViewBag.FollowingList = _context.Followers.Include(f => f.User).Where(f => f.FollowerId == currentUserId).Select(f => f.UserId).ToList();
            ViewBag.UserProfiles = userProfileDict;
            ViewBag.Comments = CommentsDict;
           // ViewData["CategoryNames"] = new SelectList(_context.BlogCategories, "Name", "Name");
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