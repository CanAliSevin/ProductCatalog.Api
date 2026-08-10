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
    public class DukkansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DukkansController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DukkansResponse>>> GetDukkans()
        {
            var dukkans = await _context.Dukkans.ToListAsync();
            var responses = dukkans.Select(d => new DukkansResponse
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
        public async Task<ActionResult<DukkansResponse>> GetDukkan(Guid id)
        {
            var dukkan = await _context.Dukkans.FindAsync(id);

            if (dukkan == null)
            {
                return NotFound();
            }

            var response = new DukkansResponse
            {
                Id = dukkan.Id,
                Name = dukkan.Name,
                Description = dukkan.Description,
                CreatedAt = dukkan.CreatedAt,
                UpdatedAt = dukkan.UpdatedAt
            };

            return Ok(response);
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(
                 [FromBody] List<Dukkan> dukkans)
        {
            _context.Dukkans.AddRange(dukkans);

            await _context.SaveChangesAsync();

            return Ok(dukkans);
        }
        [HttpPost]
        public async Task<ActionResult<DukkansResponse>> PostDukkan(CreateDukkanRequest request)
        {
            var dukkan = new Dukkan
            {
                Name = request.Name,
                Description = request.Description
            };

            _context.Dukkans.Add(dukkan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDukkan", new { id = dukkan.Id }, dukkan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDukkan(Guid id, CreateDukkanRequest request)
        {
            var dukkan = await _context.Dukkans.FindAsync(id);
            if (dukkan == null)
            {
                return NotFound();
            }

            dukkan.Name = request.Name;
            dukkan.Description = request.Description;
            dukkan.UpdatedAt = DateTime.UtcNow;

            _context.Entry(dukkan).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDukkan(Guid id)
        {
            var dukkan = await _context.Dukkans.FindAsync(id);
            if (dukkan == null)
            {
                return NotFound();

            }

            _context.Dukkans.Remove(dukkan);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}