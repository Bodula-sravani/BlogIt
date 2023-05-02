using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogIt.Models
{
    public class Comment
    {
            [Key]
            public int Id { get; set; }

            public string BlogId { get; set; }

            [ForeignKey("User")]
            public string UserId { get; set; }
            public IdentityUser User { get; set; }
            public string Content { get; set; }
            public DateTime Date { get; set; }
    }
}
