using System.Text.Json.Serialization;

namespace finalProject.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int BlogCategoryId { get; set; }
        public BlogCategory BlogCategory { get; set; }
    }
}
