using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogIt.Models
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Title { get; set; }   

        public string content { get; set; }  

        public DateTime Date { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        [ForeignKey("BlogCategory")]
        public int  CategoryId { get; set; }
        public BlogCategory BlogCategory { get; set; }

        public static explicit operator Blog(Task<Blog> v)
        {
            return v.Result;
        }
    }
}
