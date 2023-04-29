using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogIt.Models
{
    public class UserProfie
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Interests { get; set; }
        public string ProfilePic { get; set; }

        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string Youtube { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
