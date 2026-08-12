
namespace ProductCatalog.Api.DTO.Requests
{
    public class BulkCreateProductRequest
    {
        public List<CreateProductRequest> Products { get; set; } = new List<CreateProductRequest>();
    }
}