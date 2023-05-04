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
using Microsoft.AspNetCore.Identity;

namespace BlogIt.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient client;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            client = new HttpClient();
            _userManager = userManager;
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"Admin@gmail.com:Admin@1")));
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
                blogCategory = JsonConvert.DeserializeObject<BlogCategory>(data);
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
            HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:7113/api/BlogCategories", blogCategory);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(blogCategory);
            }
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
                blogCategory = JsonConvert.DeserializeObject<BlogCategory>(data);
            }
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

            // send a PUT request to the API to update the category
            HttpResponseMessage response = await client.PutAsJsonAsync($"https://localhost:7113/api/BlogCategories/{id}", blogCategory);

            // check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // redirect to the details page for the updated category
                return RedirectToAction(nameof(Details), new { id = blogCategory.Id });
            }
            else
            {
                // display an error message
                ModelState.AddModelError(string.Empty, "Unable to update the blog category. Please try again later.");
                return View();
            }
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
                blogCategory = JsonConvert.DeserializeObject<BlogCategory>(data);
            }
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
            HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7113/api/BlogCategories/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

        private bool BlogCategoryExists(int id)
        {
          return (_context.BlogCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        
    }
}
