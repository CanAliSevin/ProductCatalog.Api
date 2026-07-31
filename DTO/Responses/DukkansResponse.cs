namespace ProductCatalog.Api.DTO.Responses
{
    public class DukkansResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
