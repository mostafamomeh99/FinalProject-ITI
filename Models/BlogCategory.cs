namespace finalProject.Models
{
    public class BlogCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<BlogPost> BlogPosts { get; set; }
    }
}
