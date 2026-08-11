namespace ProductCatalog.Api.Models
{
    public class Store
    {
        public Guid Id { get; set; } = Guid.NewGuid();//storeın id'si otomatik olarak oluşturulacak ve benzersiz olacak şekilde ayarlanmıştır.

        public string Name { get; set; } = string.Empty;//storeın adı boş bir string olarak başlatılmıştır.

        public string? Description { get; set; }//storeın açıklaması boş bir string olarak başlatılmıştır.

        public ICollection<Product> Products { get; set; } = new List<Product>();// bu şu anlama geliyor ki bir store birden fazla ürüne sahip olabilir.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    }
}