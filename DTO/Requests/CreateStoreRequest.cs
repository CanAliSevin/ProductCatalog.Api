namespace ProductCatalog.Api.DTO.Requests
{
    public class CreateStoreRequest
    {
        public required string Name { get; set; } = string.Empty;
        public required string? Description { get; set; }
    }
}