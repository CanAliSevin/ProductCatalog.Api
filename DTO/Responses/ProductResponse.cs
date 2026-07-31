namespace ProductCatalog.Api.DTO.Responses
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string DukkanName { get; set; } = string.Empty;
    }
}