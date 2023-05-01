using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogIt.Models
{
    public class Follower
    {
        [Key]

        public int Id { get; set; }
        public string UserId { get; set; }

        [ForeignKey("User")]
        public string FollowerId { get; set; }     // The one who follows userId
        public IdentityUser User { get; set; }
 
    }
}

// TO see followers of x keep userId = x.Id 

//TO see following list of x keep folloerId = X.Id