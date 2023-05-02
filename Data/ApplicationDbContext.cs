using BlogIt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogIt.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfie> UserProfiles { get; set; }

        public DbSet<BlogCategory> BlogCategories { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Follower> Followers { get; set; }

        public DbSet<Comment> Comments  { get; set; }
    }
}