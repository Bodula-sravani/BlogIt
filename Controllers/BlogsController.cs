﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogIt.Data;
using BlogIt.Models;

namespace BlogIt.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Blogs.Include(b => b.BlogCategory).Include(b => b.User);
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
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            ViewData["CategoryNames"] = new SelectList(_context.BlogCategories, "Name", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
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
            _context.Add(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


            ViewData["CategoryId"] = new SelectList(_context.BlogCategories, "Id", "Id", blog.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", blog.UserId);
            return View(blog);
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
            ViewData["CategoryId"] = new SelectList(_context.BlogCategories, "Id", "Id", blog.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", blog.UserId);
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,content,Date,UserId,CategoryId")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["CategoryId"] = new SelectList(_context.BlogCategories, "Id", "Id", blog.CategoryId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", blog.UserId);
            return View(blog);
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

        private bool BlogExists(string id)
        {
          return (_context.Blogs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}