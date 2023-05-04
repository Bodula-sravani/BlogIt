using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogIt.Data;
using BlogIt.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Net.Http.Headers;
using System.Text;

namespace BlogIt.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient client;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"Admin@gmail.com:Admin@1")));
        }
        public List<BlogCategory> GetBlogCategories()
        {
            List<BlogCategory> ads = new List<BlogCategory>();
            HttpResponseMessage response = client.GetAsync("https://localhost:7113/api/BlogCategories").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var s = JsonConvert.DeserializeObject<List<BlogCategory>>(data);
                if (s != null)
                {
                    ads = s;
                }

                //Console.WriteLine("=============" + s);
            }
            return ads;
        }
        // GET: Admin
        public async Task<IActionResult> Index()
        {
              return _context.BlogCategories != null ? 
                          View(GetBlogCategories()) :
                          Problem("Entity set 'ApplicationDbContext.BlogCategories'  is null.");
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BlogCategories == null)
            {
                return NotFound();
            }
            HttpResponseMessage response = client.GetAsync($"https://localhost:7113/api/BlogCategories/{id}").Result;
            BlogCategory blogCategory = null;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var s = JsonConvert.DeserializeObject<BlogCategory>(data);
                blogCategory = s;
            }
            if (blogCategory == null)
            {
                return NotFound();
            }

            return View(blogCategory);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] BlogCategory blogCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogCategory);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogCategories == null)
            {
                return NotFound();
            }

            var blogCategory = await _context.BlogCategories.FindAsync(id);
            if (blogCategory == null)
            {
                return NotFound();
            }
            return View(blogCategory);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] BlogCategory blogCategory)
        {
            if (id != blogCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogCategoryExists(blogCategory.Id))
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
            return View(blogCategory);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogCategories == null)
            {
                return NotFound();
            }

            var blogCategory = await _context.BlogCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogCategory == null)
            {
                return NotFound();
            }

            return View(blogCategory);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogCategories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogCategories'  is null.");
            }
            var blogCategory = await _context.BlogCategories.FindAsync(id);
            if (blogCategory != null)
            {
                _context.BlogCategories.Remove(blogCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogCategoryExists(int id)
        {
          return (_context.BlogCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
