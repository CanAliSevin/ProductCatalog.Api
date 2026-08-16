using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.DTO.Requests;
using ProductCatalog.Api.DTO.Responses;
using ProductCatalog.Api.Models;
using ProductCatalog.Api.Services;


namespace ProductCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmbeddingService _embeddingService;
        private readonly ProductSearchService _searchService;


        public ProductsController(
            ApplicationDbContext context,
            IEmbeddingService embeddingService,
            ProductSearchService searchService)
        {
            _context = context;
            _embeddingService = embeddingService;
            _searchService = searchService;
        }

        private static ProductResponse ToResponse(Product product) => new()
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
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> SearchProducts(
    [FromQuery] string q,
    [FromQuery] int limit = 5)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new { message = "Arama sorgusu boş olamaz." });
            }

            var results = await _searchService.SearchAsync(q, limit);
            return Ok(results);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts([FromQuery] Guid? categoryId = null)

        {
            var query = _context.Products
                  .Include(p => p.Category)
                  .Include(p => p.store)
                  .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query.ToListAsync();
            return Ok(products.Select(ToResponse));
        }

        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsAdmin()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.store)
                .ToListAsync();

            return Ok(products.Select(ToResponse));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.store)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(ToResponse(product));
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(BulkCreateProductRequest request)
        {
            var products = new List<Product>();

            foreach (var item in request.Products)
            {
                var embeddingText = $"{item.Name}. {item.Description}";
                var embedding = await _embeddingService.GetEmbeddingAsync(embeddingText);

                products.Add(new Product
                {
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    StoreId = item.StoreId,
                    CategoryId = item.CategoryId,
                    ImageUrl = item.ImageUrl,
                    Embedding = embedding
                });
            }

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            return Ok(products.Select(ToResponse));
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> PostProduct(CreateProductRequest request)
        {
            var embeddingText = $"{request.Name} {request.Description}";
            var embedding = await _embeddingService.GetEmbeddingAsync(embeddingText);

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StoreId = request.StoreId,
                CategoryId = request.CategoryId,
                ImageUrl = request.ImageUrl,
                Embedding = embedding
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            await _context.Entry(product).Reference(p => p.store).LoadAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, ToResponse(product));
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

            var embeddingText = $"{request.Name} {request.Description}";
            product.Embedding = await _embeddingService.GetEmbeddingAsync(embeddingText);

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

        [HttpPost("generate-embeddings")]
        public async Task<IActionResult> GenerateEmbeddings()
        {
            var products = await _context.Products
                .Where(p => p.Embedding == null)
                .ToListAsync();

            if (products.Count == 0)
            {
                return Ok(new { message = "Embedding oluşturulacak ürün bulunamadı." });
            }

            foreach (var product in products)
            {
                var embeddingText = $"{product.Name}. {product.Description}";
                product.Embedding = await _embeddingService.GetEmbeddingAsync(embeddingText);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Embedding'ler başarıyla oluşturuldu.", count = products.Count });
        }
    }
}