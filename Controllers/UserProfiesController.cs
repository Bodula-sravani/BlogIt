﻿using System;
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

namespace BlogIt.Controllers
{
    public class UserProfiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> userManager;
        // private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserProfiesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)//, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            //this.roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: UserProfies
        public async Task<IActionResult> Index()
        {

            var userId = userManager.GetUserId(this.User);

            var user = await userManager.FindByIdAsync(userId);

         //   var roles = await userManager.GetRolesAsync(user);

            var userProfile = _context.UserProfiles.Include(u => u.User).FirstOrDefault(u => u.UserId == userId);
            //userProfile.Name = userProfile.User.UserName;
            userProfile.Email = userProfile.User.Email;

          //  var applicationDbContext = _context.UserProfiles.Include(u => u.User);
            return View(userProfile);
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

        // GET: UserProfies/Create
        public async Task<IActionResult> Create()
        {
            var userId = userManager.GetUserId(this.User);

            var user = await userManager.FindByIdAsync(userId);

            //   var roles = await userManager.GetRolesAsync(user);

            var userProfile = _context.UserProfiles.Include(u => u.User).FirstOrDefault(u => u.UserId == userId);

            if (userProfile == null)
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
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
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Description,Interests,ProfilePic,UserId")] UserProfie userProfie, IFormFile profilePic)
        {

            var userId = userManager.GetUserId(this.User);

            var user = await userManager.FindByIdAsync(userId);

            var Email = await userManager.GetEmailAsync(user);


            //   var roles = await userManager.GetRolesAsync(user);
            userProfie.UserId = userId;
            userProfie.Email = Email;
            if (profilePic != null && profilePic.Length > 0)
            {
                // Generate a unique filename for the profile picture
                string fileName = Path.GetFileName(profilePic.FileName);
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
                    await profilePic.CopyToAsync(stream);
                }

                // Update the user profile object with the URL of the saved image
                userProfie.ProfilePic = "/images/" + uniqueFileName;
            }
            //var userProfileTemp = _context.UserProfiles.Include(u => u.User).FirstOrDefault(u => u.UserId == userId);
            //userProfile.Name = userProfileTemp.User.UserName;
            //userProfile.Email = userProfileTemp.User.Email;
            _context.Add(userProfie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userProfie.UserId);
            return View(userProfie);
        }

        // GET: UserProfies/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.UserProfiles == null)
            {
                return NotFound();
            }
            var userId = userManager.GetUserId(this.User);

            var user = await userManager.FindByIdAsync(userId);

            //   var roles = await userManager.GetRolesAsync(user);

            var userProfile = _context.UserProfiles.Include(u => u.User).FirstOrDefault(u => u.UserId == userId);
            //userProfile.Name = userProfile.User.UserName;
            userProfile.Email = userProfile.User.Email;
            //var userProfie = await _context.UserProfiles.FindAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userProfile.UserId); ;
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
                    userProfile.ProfilePic = "/images/" + uniqueFileName;
                }
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userProfile.UserId);
            return View(userProfile);
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
