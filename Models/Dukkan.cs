namespace ProductCatalog.Api.Models
{
    public class Dukkan
    {
        public Guid Id { get; set; } = Guid.NewGuid();//dukkanın id'si otomatik olarak oluşturulacak ve benzersiz olacak şekilde ayarlanmıştır.

        public string Name { get; set; } = string.Empty;//dukkanın adı boş bir string olarak başlatılmıştır.

        public string? Description { get; set; }//dukkanın açıklaması boş bir string olarak başlatılmıştır.

        public ICollection<Product> Products { get; set; } = new List<Product>();// bu şu anlama geliyor ki bir dukkan birden fazla ürüne sahip olabilir.

    }
}