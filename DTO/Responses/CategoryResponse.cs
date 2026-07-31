namespace ProductCatalog.Api.DTO.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
    }
}