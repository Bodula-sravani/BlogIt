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

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var currentUserId = userManager.GetUserId(this.User);

            var currentUser = await userManager.FindByIdAsync(currentUserId);

            // Get all blogs of current user and display it in his page
            var applicationDbContext = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.User).Where(b=>b.User.Id==currentUserId).OrderByDescending(b => b.Date);

            // To store userProfiles of that blog id, to display userName and user profile pic in each comment
            var userProfileDict = new Dictionary<string, UserProfie>();

            // To store Comments of that blog id, to display userName and user profile pic in blogs
            var CommentsDict = new Dictionary<string, List<Comment>>();
            foreach (var blog in applicationDbContext)
            {

                var comments = await _context.Comments.Where(c => c.BlogId == blog.Id).ToListAsync();

                CommentsDict[blog.Id] = comments;
                foreach (var comment in comments)
                {
                    var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == comment.UserId);
                    userProfileDict[comment.UserId] = (UserProfie)userProfile;

                }

            }
        
            ViewBag.UserProfiles = userProfileDict;
            ViewBag.Comments = CommentsDict;
            return View(await applicationDbContext.ToListAsync());
            
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
            // To store Comments of that blog id, to display userName and user profile pic in blogs
            var CommentsDict = new Dictionary<string, List<Comment>>();
            var comments = await _context.Comments.Where(c => c.BlogId == blog.Id).ToListAsync();
            CommentsDict[blog.Id] = comments;

            // To store userProfiles of that blog id, to display userName and user profile pic in each comment
            var userProfileDict = new Dictionary<string, UserProfie>();
            foreach (var comment in comments)
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == comment.UserId);
                userProfileDict[comment.UserId] = (UserProfie)userProfile;

            }
            ViewBag.UserProfiles = userProfileDict;
            ViewBag.Comments = CommentsDict;
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }
        //public Blog SetImageInBlog(Blog blog, string CategoryName, string EditorContent)
        //{
        //    //To get the src url and store it in the local directory
        //    string imgSrc = "";
        //    string htmlString = EditorContent;
        //    int startIndex = htmlString.IndexOf("<img"); // find the start index of img tag
        //    if (startIndex >= 0) // if img tag exists
        //    {
        //        int endIndex = htmlString.IndexOf(">", startIndex); // find the end index of img tag
        //        if (endIndex >= 0)
        //        {
        //            string imgTag = htmlString.Substring(startIndex, endIndex - startIndex + 1); // extract the img tag
        //            int srcIndex = imgTag.IndexOf("src=\""); // find the start index of src attribute
        //            if (srcIndex >= 0)
        //            {
        //                int srcEndIndex = imgTag.IndexOf("\"", srcIndex + 5); // find the end index of src attribute
        //                if (srcEndIndex >= 0)
        //                {
        //                    imgSrc = imgTag.Substring(srcIndex + 5, srcEndIndex - srcIndex - 5); // extract the image URL
        //                    string destinationFilePath = SaveImage(imgSrc).Result;
        //                    EditorContent.Replace(imgSrc, destinationFilePath);
        //                }
        //            }
        //        }
        //    }

        //    blog.Date = DateTime.Now;
        //        blog.CategoryId = _context.BlogCategories.Where(c => c.Name == CategoryName).Select(c => c.Id).FirstOrDefault();
        //        blog.content = EditorContent;
        //        return blog;
        //    }


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
        public async Task<IActionResult> Create([Bind("Id,Title,content,Date,UserId,CategoryId")] Blog blog,string CategoryName,string EditorContent)
        {
            blog.Date = DateTime.Now;
            blog.CategoryId = _context.BlogCategories .Where(c => c.Name == CategoryName).Select(c => c.Id).FirstOrDefault();
            blog.content= EditorContent;
            blog.UserId= userManager.GetUserId(this.User);
            _context.Add(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,content,Date,UserId,CategoryId")] Blog blog,string CategoryName,string EditorContent)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            try
            {
                blog.Date = DateTime.Now;
                blog.CategoryId = _context.BlogCategories.Where(c => c.Name == CategoryName).Select(c => c.Id).FirstOrDefault();
                blog.content = EditorContent;
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


