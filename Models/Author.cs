using System.Text.Json.Serialization;

namespace finalProject.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Bio { get; set; }
        public string? ImageUrl { get; set; }
      
        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}
