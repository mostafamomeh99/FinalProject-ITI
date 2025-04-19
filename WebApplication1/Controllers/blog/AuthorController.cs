

using DatabaseConnection;
using finalProject.DTOs;
using finalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace finalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AuthorDto>> GetAll()
        {
            var authors =  _context.Authors
                .Select(a => new AuthorDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Bio = a.Bio,
                    ImageUrl = a.ImageUrl
                }).ToList();

            return Ok(authors);
        }

        [HttpPost]
        public ActionResult Create(AuthorDto dto)
        {
            var author = new Author
            {
                Name = dto.Name,
                Bio = dto.Bio,
                ImageUrl = dto.ImageUrl
            };

            _context.Authors.Add(author);
             _context.SaveChanges();
            return Ok();
        }



        [HttpGet("{id}")]
        public ActionResult<AuthorDto> GetById(int id)
        {
            var author =  _context.Authors.Find(id);
            if (author == null) return NotFound();

            return Ok(new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio,
                ImageUrl = author.ImageUrl
            });
        }



        [HttpPut("{id}")]
        public IActionResult Update(int id, AuthorDto dto)
        {
            var author =  _context.Authors.Find(id);
            if (author == null) return NotFound();

            author.Name = dto.Name;
            author.Bio = dto.Bio;
            author.ImageUrl = dto.ImageUrl;

             _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var author = _context.Authors.Find(id);
            if (author == null) return NotFound();

            _context.Authors.Remove(author);
             _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("{id}/posts")]
        public IActionResult GetAuthorPosts(int id)
        {
            var posts = _context.BlogPosts
                .Where(p => p.AuthorId == id)
                .Select(p => new BlogPostAuthorDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    ImageUrl = p.ImageUrl,
                    CreatedAt = p.CreatedAt
                })
                .ToList();

            return Ok(posts);
        }
    }
}
