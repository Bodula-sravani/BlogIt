using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogIt.Data;
using BlogIt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace BlogIt.Controllers
{
    public class UserProfiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserProfiesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: UserProfies
        public async Task<IActionResult> Index()
        {

            var currentUserId = userManager.GetUserId(this.User);

            var currentUser = await userManager.FindByIdAsync(currentUserId);

          // Check if the user is logged in as an admin
                if (User.IsInRole("Admin"))
                {
                    // Create a new HttpClient instance
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(30);
                        // Set the base address of the API endpoint
                        client.BaseAddress = new Uri("http://localhost:7113/api/BlogCategories");

                    // Call the API endpoint and get the response
                    //HttpResponseMessage response = client.GetAsync("GetAllCategories").Result;
                    try
                    {
                        // Call the API endpoint and get the response
                        HttpResponseMessage response = await client.GetAsync("");

                        // Check if the response is successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the response content
                            var content = await response.Content.ReadAsStringAsync();
                            var categories = JsonConvert.DeserializeObject<List<BlogCategory>>(content);

                            // Do something with the categories data
                            return View(categories);
                        }
                        else
                        {
                            // Handle the error response
                            return View("Error");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception
                        return View("Error");
                    }
                }
                }
            else
            {
                // Getting the current userProfile details
                var userProfile = _context.UserProfiles.Include(u => u.User).FirstOrDefault(u => u.UserId == currentUserId);
                userProfile.Email = userProfile.User.Email;
                
                // To display the latest 2 posts of that user in his profile page
                var BlogListTop2 = _context.Blogs
                                                .Include(b => b.BlogCategory)
                                                .Include(b => b.User)
                                                .Where(b => b.User.Id == currentUserId)
                                                .OrderByDescending(b => b.Date)
                                                .Take(2).ToList();

                ViewBag.BlogListTop2 = BlogListTop2.Count==0 ? null: BlogListTop2;
                return View(userProfile);
            }
            
        }

        // GET: UserProfies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.UserProfiles == null)
            {
                return NotFound();
            }

            var userProfie = await _context.UserProfiles
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProfie == null)
            {
                return NotFound();
            }

            return View(userProfie);
        }
        public async Task<UserProfie> SetImageAndProfile(UserProfie userProfie, IFormFile ProfilePic)
        {
            // To store the values in userProfile model save the image in wwwroot/Images folder  - used in edit & create user
            var currentUserId = userManager.GetUserId(this.User);

            var currentUser = await userManager.FindByIdAsync(currentUserId);

            var currentUserEmail = await userManager.GetEmailAsync(currentUser);

            userProfie.UserId = currentUserId;
            userProfie.Email = currentUserEmail;
            if (ProfilePic != null && ProfilePic.Length > 0)
            {
                // Generate a unique filename for the profile picture
                string fileName = Path.GetFileName(ProfilePic.FileName);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

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
                    await ProfilePic.CopyToAsync(stream);
                }

                // Update the user profile object with the URL of the saved image
                userProfie.ProfilePic = "/images/" + uniqueFileName;
            }
            return userProfie;
        }


        // GET: UserProfies/Create
        public async Task<IActionResult> Create()
        {
            // Intinally comes to create profile page if user profile is already created goes to index pages
            var currentUserId = userManager.GetUserId(this.User);
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            var userProfile = _context.UserProfiles.Include(u => u.User).FirstOrDefault(u => u.UserId == currentUserId);

            if (userProfile == null)
            {
                // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
                ViewBag.UserId = currentUserId;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: UserProfies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Description,Interests,ProfilePic,UserId,Facebook,Twitter,Instagram,Youtube")] UserProfie userProfie, IFormFile ProfilePic)
        {
            // Sets all fields values of the profile and saves the profile pic 
            userProfie = (UserProfie)SetImageAndProfile(userProfie, ProfilePic);
            _context.Add(userProfie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: UserProfies/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.UserProfiles == null)
            {
                return NotFound();
            }
            var currentUserId = userManager.GetUserId(this.User);

            var currentUser = await userManager.FindByIdAsync(currentUserId);

            var userProfile = _context.UserProfiles.Include(u => u.User).FirstOrDefault(u => u.UserId == currentUserId);
            if (userProfile == null)
            {
                return NotFound();
            }
            userProfile.Email = userProfile.User.Email;
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userProfile.UserId); ;
            ViewBag.UserId = currentUserId;
            return View(userProfile);
        }

        // POST: UserProfies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Email,Description,Interests,ProfilePic,UserId,Facebook,Twitter,Instagram,Youtube")] UserProfie userProfile, IFormFile ProfilePic)
        {
            if (id != userProfile.Id)
            {
                return NotFound();
            }
            try
            {
                userProfile = (UserProfie)SetImageAndProfile(userProfile, ProfilePic);
                _context.Update(userProfile);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfieExists(userProfile.Id))
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

        // GET: UserProfies/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.UserProfiles == null)
            {
                return NotFound();
            }

            var userProfie = await _context.UserProfiles
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProfie == null)
            {
                return NotFound();
            }

            return View(userProfie);
        }

        // POST: UserProfies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.UserProfiles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UserProfiles'  is null.");
            }
            var userProfie = await _context.UserProfiles.FindAsync(id);
            if (userProfie != null)
            {
                _context.UserProfiles.Remove(userProfie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserProfieExists(string id)
        {
          return (_context.UserProfiles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
