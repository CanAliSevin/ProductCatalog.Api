using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.DTO.Requests;
using ProductCatalog.Api.DTO.Responses;
namespace ProductCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            var responses = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name
            });
            return Ok(responses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> PostCategory(CreateCategoriesRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name
            };

            return CreatedAtAction("GetCategory", new { id = category.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(Guid id, CreateCategoriesRequest request)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = request.Name;
            await _context.SaveChangesAsync();

            var response = new CategoryResponse
            {
                Name = category.Name
            };

            return Ok(response);
        }


    }
}