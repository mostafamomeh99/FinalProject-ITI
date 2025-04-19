using DatabaseConnection;

using finalProject.DTOs;
using finalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace finalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public  ActionResult<IEnumerable<BlogCategoryDto>> GetAll()
        {
            var categories = _context.BlogCategories
                .Select(c => new BlogCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

            return Ok(categories);
        }

        [HttpPost]
        public  ActionResult Create(BlogCategoryDto dto)
        {
            var category = new BlogCategory
            {
                Name = dto.Name
            };

            _context.BlogCategories.Add(category);
             _context.SaveChanges();
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<BlogCategoryDto> GetById(int id)
        {
            var category =  _context.BlogCategories.Find(id);
            if (category == null) return NotFound();

            return Ok(new BlogCategoryDto
            {
                Id = category.Id,
                Name = category.Name
            });
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, BlogCategoryDto dto)
        {
            var category = _context.BlogCategories.Find(id);
            if (category == null) return NotFound();

            category.Name = dto.Name;
             _context.SaveChanges();
            return Ok();
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var category =_context.BlogCategories.Find(id);
            if (category == null) return NotFound();

            _context.BlogCategories.Remove(category);
             _context.SaveChanges();
            return Ok();
        }


        [HttpGet("{id}/posts")]
        public IActionResult GetPostsByCategory(int id)
        {
            var posts = _context.BlogPosts
                .Where(p => p.BlogCategoryId == id)
                .Select(p => new BlogPostCategoryDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    ImageUrl = p.ImageUrl,
                    CreatedAt = p.CreatedAt,
                    BlogCategoryId = p.BlogCategoryId,
                    BlogCategoryName = p.BlogCategory.Name 
                })
                .ToList();

            return Ok(posts);
        }
    }
}
