using System.ComponentModel.DataAnnotations;

namespace BlogIt.Models
{
    public class BlogCategory
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
