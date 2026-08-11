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
    public class StoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StoresController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoresResponse>>> GetStores()
        {
            var stores = await _context.Stores.ToListAsync();
            var responses = stores.Select(d => new StoresResponse
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            });
            return Ok(responses);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<StoresResponse>> GetStore(Guid id)
        {
            var store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            var response = new StoresResponse
            {
                Id = store.Id,
                Name = store.Name,
                Description = store.Description,
                CreatedAt = store.CreatedAt,
                UpdatedAt = store.UpdatedAt
            };

            return Ok(response);
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(
                 [FromBody] List<Store> stores)
        {
            _context.Stores.AddRange(stores);

            await _context.SaveChangesAsync();

            return Ok(stores);
        }
        [HttpPost]
        public async Task<ActionResult<StoresResponse>> PostStore(CreateStoreRequest request)
        {
            var store = new Store
            {
                Name = request.Name,
                Description = request.Description
            };

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStore", new { id = store.Id }, store);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore(Guid id, CreateStoreRequest request)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            store.Name = request.Name;
            store.Description = request.Description;
            store.UpdatedAt = DateTime.UtcNow;

            _context.Entry(store).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(Guid id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();

            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}