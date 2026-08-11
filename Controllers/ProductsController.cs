using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.DTO.Requests;
using ProductCatalog.Api.DTO.Responses;
using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.store)
                .ToListAsync();

            var responses = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                StoreId = p.StoreId,
                CategoryName = p.Category?.Name ?? "",
                StoreName = p.store?.Name ?? "",
                ImageUrl = p.ImageUrl
            });

            return Ok(responses);
        }
        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsAdmin()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.store)
                .ToListAsync();

            var responses = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                StoreId = p.StoreId,
                CategoryName = p.Category?.Name ?? "",
                StoreName = p.store?.Name ?? "",
                ImageUrl = p.ImageUrl
            });

            return Ok(responses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(Guid id)
        {
            var product = await _context.Products
            /*
            CategoryName ve StoreName erişebilmek için .Include kullanıyourz. 
            yani sınıflar arasında baglantı kuruyoruz
            */
                .Include(p => p.Category)
                .Include(p => p.store)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                StoreId = product.StoreId,
                CategoryName = product.Category?.Name ?? "",
                StoreName = product.store?.Name ?? "",
                ImageUrl = product.ImageUrl
            };

            return Ok(response);
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(
                   [FromBody] List<Product> products)
        {
            _context.Products.AddRange(products);

            await _context.SaveChangesAsync();

            return Ok(products);
        }
        [HttpPost]
        public async Task<ActionResult<ProductResponse>> PostProduct(CreateProductRequest request)
        {
            // Request -> Entity
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StoreId = request.StoreId,
                CategoryId = request.CategoryId,
                ImageUrl = request.ImageUrl
            };
            Console.WriteLine($"Product created with ID: {product.Id}");
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // İlişkili verileri yükle
            await _context.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();

            await _context.Entry(product)
                .Reference(p => p.store)
                .LoadAsync();

            // Entity -> Response
            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = product.Category?.Name ?? "",
                StoreName = product.store?.Name ?? "",
                ImageUrl = product.ImageUrl
            };

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(Guid id, CreateProductRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.StoreId = request.StoreId;
            product.CategoryId = request.CategoryId;
            product.ImageUrl = request.ImageUrl;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}