using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.DTO.Responses;
using ProductCatalog.Api.Models;
using Pgvector.EntityFrameworkCore;
namespace ProductCatalog.Api.Services;

public class ProductSearchService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmbeddingService _embeddingService;

    public ProductSearchService(
        ApplicationDbContext context,
        IEmbeddingService embeddingService)
    {
        _context = context;
        _embeddingService = embeddingService;
    }

    public async Task<List<ProductResponse>> SearchAsync(
        string query,
        int limit = 5)
    {
        // 1. Kullanıcının arama metnini embedding'e çevir
        var queryEmbedding =
            await _embeddingService.GetEmbeddingAsync(query);

        // 2. PostgreSQL + pgvector ile benzer ürünleri bul
        var products = await _context.Products
            .Where(p => p.Embedding != null)
            .OrderBy(p => p.Embedding!.CosineDistance(queryEmbedding))
            .Take(limit)
            .Include(p => p.Category)
            .Include(p => p.store)
            .ToListAsync();

        // 3. Entity -> Response
        return products.Select(p => new ProductResponse
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
        }).ToList();
    }
}