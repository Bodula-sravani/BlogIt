using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogIt.Data;
using BlogIt.Models;

namespace BlogIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BlogCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogCategory>>> GetBlogCategories()
        {
          if (_context.BlogCategories == null)
          {
              return NotFound();
          }
            return await _context.BlogCategories.ToListAsync();
        }

        // GET: api/BlogCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogCategory>> GetBlogCategory(int id)
        {
          if (_context.BlogCategories == null)
          {
              return NotFound();
          }
            var blogCategory = await _context.BlogCategories.FindAsync(id);

            if (blogCategory == null)
            {
                return NotFound();
            }

            return blogCategory;
        }

        // PUT: api/BlogCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogCategory(int id, BlogCategory blogCategory)
        {
            if (id != blogCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(blogCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BlogCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BlogCategory>> PostBlogCategory(BlogCategory blogCategory)
        {
          if (_context.BlogCategories == null)
          {
              return Problem("Entity set 'ApplicationDbContext.BlogCategories'  is null.");
          }
            _context.BlogCategories.Add(blogCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogCategory", new { id = blogCategory.Id }, blogCategory);
        }

        // DELETE: api/BlogCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogCategory(int id)
        {
            if (_context.BlogCategories == null)
            {
                return NotFound();
            }
            var blogCategory = await _context.BlogCategories.FindAsync(id);
            if (blogCategory == null)
            {
                return NotFound();
            }

            _context.BlogCategories.Remove(blogCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogCategoryExists(int id)
        {
            return (_context.BlogCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
