namespace finalProject.DTOs
{
    public class BlogPostCategoryDto
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int BlogCategoryId { get; set; }
        public string? BlogCategoryName { get; set; }
    }
}
