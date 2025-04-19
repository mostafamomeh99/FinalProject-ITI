using DatabaseConnection;

using finalProject.DTOs;
using finalProject.Helper;
using finalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace finalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BlogPostDto>> GetAll(
    [FromQuery] PaginationParams pagination,
    [FromQuery] string? search,
    [FromQuery] int? categoryId)
        {
            var query = _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.BlogCategory)
                .AsQueryable();

            // 🔎 فلترة البحث
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Title.Contains(search) ||
                    p.Content.Contains(search));
            }

            // 🧩 فلترة الكاتيجوري
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.BlogCategoryId == categoryId.Value);
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pagination.PageNumber - 1) * pagination.ValidatedPageSize)
                .Take(pagination.ValidatedPageSize)
                .Select(p => new BlogPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    ImageUrl = p.ImageUrl,
                    CreatedAt = p.CreatedAt,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Name,
                    BlogCategoryId = p.BlogCategoryId,
                    BlogCategoryName = p.BlogCategory.Name
                })
                .ToList();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                Data = items
            });
        }

        [HttpGet("last")]
        public ActionResult<IEnumerable<BlogPostDto>> GetLastPosts()
        {
            var lastPosts = _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.BlogCategory)
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .Select(p => new BlogPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    ImageUrl = p.ImageUrl,
                    CreatedAt = p.CreatedAt,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.Name,
                    BlogCategoryId = p.BlogCategoryId,
                    BlogCategoryName = p.BlogCategory.Name
                })
                .ToList();

            return Ok(lastPosts);
        }


        [HttpGet("{id}")]
        public ActionResult<BlogPostDto> GetById(int id)
        {
            var post = _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.BlogCategory)
                .FirstOrDefault(p => p.Id == id);

            if (post == null) return NotFound();

            return Ok(new BlogPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.Name,
                BlogCategoryId = post.BlogCategoryId,
                BlogCategoryName = post.BlogCategory.Name
            });
        }

        [HttpPost]
        public ActionResult Create(BlogPostDto dto)
        {
            var post = new BlogPost
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                AuthorId = dto.AuthorId,
                BlogCategoryId = dto.BlogCategoryId
            };

            _context.BlogPosts.Add(post);
             _context.SaveChanges();
            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, BlogPostDto dto)
        {
            var post = _context.BlogPosts.Find(id);
            if (post == null) return NotFound();

            post.Title = dto.Title;
            post.Content = dto.Content;
            post.ImageUrl = dto.ImageUrl;
            post.AuthorId = dto.AuthorId;
            post.BlogCategoryId = dto.BlogCategoryId;

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var post =  _context.BlogPosts.Find(id);
            if (post == null) return NotFound();

            _context.BlogPosts.Remove(post);
             _context.SaveChanges();
            return Ok();
        }
    }
}
