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

namespace BlogIt.Controllers
{
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
            var userId = _userManager.GetUserId(this.User);
            //var user = await _userManager.FindByIdAsync(userId);
            var applicationDbContext = _context.Followers.Include(f => f.User).Where(f=>f.UserId == userId);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Following()
        {
            
            var userId = _userManager.GetUserId(this.User);
            var user = await _userManager.FindByIdAsync(userId);
            var applicationDbContext = _context.Followers.Include(f => f.User).Where(f => f.FollowerId == userId);
            // Getting names of following users since I am not storing following userID as user foreignKey 
            // Only Follower userId is refrencing to User as Foreign key
            foreach(var item in  await applicationDbContext.ToListAsync())
            {
                var itemUser = await _userManager.FindByIdAsync(item.UserId);
                //followingUsers.Add(itemUser);
                ViewData[item.UserId] = itemUser.UserName;
            }


            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Followers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Followers == null)
            {
                return NotFound();
            }

            var follower = await _context.Followers
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (follower == null)
            {
                return NotFound();
            }

            return View(follower);
        }
        public async Task<IActionResult> Index(string userId)
        {
            // Using index to create a record in table
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
