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
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BlogIt.Controllers
{
    [Authorize(Roles = "User")]
    public class FollowersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public FollowersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Followers
        public async Task<IActionResult> Followers()
        {
            // TO get followers of current user
            var currentUserId = _userManager.GetUserId(this.User);
            var applicationDbContext = _context.Followers.Include(f => f.User).Where(f=>f.UserId == currentUserId);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Following()
        {
            // To get the following list of current user
            var currentUserId = _userManager.GetUserId(this.User);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var applicationDbContext = _context.Followers.Include(f => f.User).Where(f => f.FollowerId == currentUserId);

            // Getting names of following users since I am not storing followingUserID as user foreignKey 
            // Only FollowerserId is refrencing to User as Foreign key
            foreach(var item in  await applicationDbContext.ToListAsync())
            {
                var itemUser = await _userManager.FindByIdAsync(item.UserId);
                //followingUsers.Add(itemUser);
                ViewData[item.UserId] = itemUser.UserName;
            }


            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Index(string userId)
        {
            // Using index to create a record in Followers table
            var currentUserId = _userManager.GetUserId(this.User);
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "Id");
            var Follower = new Follower();
            Follower.UserId = userId;
            Follower.FollowerId = currentUserId;
            _context.Add(Follower);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }

        // GET: Followers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // To remove a follower or to unfollow a following user and redirects to profile page
            if (id == null || _context.Followers == null)
            {
                return NotFound();
            }
            var follower = await _context.Followers.FindAsync(id);
            if (follower != null)
            {
                _context.Followers.Remove(follower);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index","UserProfies");
        }


        private bool FollowerExists(int id)
        {
          return (_context.Followers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
