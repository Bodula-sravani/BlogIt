using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogIt.Data;
using BlogIt.Models;
using Microsoft.AspNetCore.Html;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using HtmlAgilityPack;
using System.Reflection.Metadata;
using System.IO;
using System.Web;
using System.Data;

namespace BlogIt.Controllers
{
    [Authorize(Roles ="User")]
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var currentUserId = userManager.GetUserId(this.User);

            var currentUser = await userManager.FindByIdAsync(currentUserId);

            // Get all blogs of current user and display it in his page
            var blogs = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.User).Where(b=>b.User.Id==currentUserId).OrderByDescending(b => b.Date).ToList();

            // To store userProfiles of that blog id, to display userName and user profile pic in each comment
            var userProfileDict = new Dictionary<string, UserProfie>();

            // To store Comments of that blog id, to display userName and user profile pic in blogs
            var CommentsDict = new Dictionary<string, List<Comment>>();
            
            await GetData(blogs, userProfileDict, CommentsDict);
        
            ViewBag.UserProfiles = userProfileDict;
            ViewBag.Comments = CommentsDict;
            return View(blogs);
            
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }
  
            var blog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(blog == null)
            {
                return NotFound();
            }

            // To store Comments of that blog id
            var CommentsDict = new Dictionary<string, List<Comment>>();
            var comments = await _context.Comments.Where(c => c.BlogId == blog.Id).ToListAsync();
            CommentsDict[blog.Id] = comments;

            // To store userProfiles of that blog id, to display userName and user profile pic in each comment
            var userProfileDict = new Dictionary<string, UserProfie>();
            foreach (var comment in comments)
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == comment.UserId);
                if(userProfile!=null) userProfileDict[comment.UserId] = (UserProfie)userProfile;
            }
            ViewBag.UserProfiles = userProfileDict;
            ViewBag.Comments = CommentsDict;
            return View(blog);
        }
        public async Task<string> SaveImage(IFormFile Pic)
        {
            string ImgPath = "";

            if (Pic != null && Pic.Length > 0)
            {
                // Generate a unique filename for the profile picture
                string fileName = Path.GetFileName(Pic.FileName);
                //string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                string uniqueFileName = fileName;
                // Get the web root path
                string webRootPath = _webHostEnvironment.WebRootPath;

                // Combine the web root path and the "images" folder to get the full path to the destination folder
                string destinationFolderPath = Path.Combine(webRootPath, "images");

                // If the destination folder doesn't exist, create it
                if (!Directory.Exists(destinationFolderPath))
                {
                    Directory.CreateDirectory(destinationFolderPath);
                }

                // Combine the destination folder path and the unique filename to get the full path to the destination file
                string destinationFilePath = Path.Combine(destinationFolderPath, uniqueFileName);

                // Save the file to the destination path
                using (var stream = new FileStream(destinationFilePath, FileMode.Create))
                {
                    await Pic.CopyToAsync(stream);
                }

                // Update the user profile object with the URL of the saved image
                ImgPath = $"<img src=\"/images/{uniqueFileName}\" alt=\"Picture\" style=\"display: block; margin: 0 auto;\"/>";

            }
            return ImgPath;
        }
        // GET: Blogs/Create
        public IActionResult Create()
        {
            var currentUserId = userManager.GetUserId(this.User);
            ViewData["CategoryNames"] = new SelectList(_context.BlogCategories, "Name", "Name");
            ViewBag.UserId = currentUserId;
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,content,Date,UserId,CategoryId")] Blog blog,string CategoryName,string EditorContent, IFormFile Pic)
        {
            try
            {
                blog.Date = DateTime.Now;
                blog.CategoryId = _context.BlogCategories.Where(c => c.Name == CategoryName).Select(c => c.Id).FirstOrDefault();
                blog.content = SaveImage(Pic).Result + EditorContent;
                blog.UserId = userManager.GetUserId(this.User);
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(DBConcurrencyException)
            {
                return NotFound();
            }
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            ViewData["content"] = blog.content;
            ViewData["CategoryNames"] = new SelectList(_context.BlogCategories, "Name", "Name");
            ViewData["CurrentCategory"] = _context.BlogCategories.Where(c => c.Id == blog.CategoryId).Select(c => c.Name).FirstOrDefault();
            ViewBag.UserId = userManager.GetUserId(this.User);
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,content,Date,UserId,CategoryId")] Blog blog,string CategoryName,string EditorContent, IFormFile Pic)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            try
            {
                
                blog.Date = DateTime.Now;
                blog.CategoryId = _context.BlogCategories.Where(c => c.Name == CategoryName).Select(c => c.Id).FirstOrDefault();
                blog.content = SaveImage(Pic).Result + EditorContent;
                blog.UserId = userManager.GetUserId(this.User);
                _context.Update(blog);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(blog.Id))
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

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.BlogCategory)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Blogs'  is null.");
            }
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(string blogId,string content)
        {
            var blog = await _context.Blogs.FindAsync(blogId);
            if (blog == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                BlogId = blogId,
                UserId = user.Id,
                Content = content,
                Date = DateTime.Now
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Home");
        }
        private bool BlogExists(string id)
        {
          return (_context.Blogs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}


