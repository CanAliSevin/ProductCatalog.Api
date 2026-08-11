namespace ProductCatalog.Api.DTO.Requests
{
    public class CreateProductRequest
    {
        public required string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public required decimal Price { get; set; }
        public required Guid StoreId { get; set; }
        public required Guid CategoryId { get; set; }

        public string? ImageUrl { get; set; }
    }
}