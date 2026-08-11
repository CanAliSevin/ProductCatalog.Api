namespace ProductCatalog.Api.DTO.Responses
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
    }
}