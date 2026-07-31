namespace ProductCatalog.Api.Models
{
    public class Category
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public Guid? ParentCategoryId { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();// bu şu anlama geliyor ki bir kategori birden fazla ürüne sahip olabilir. Bu nedenle, Category sınıfında Products adında bir ICollection<Product> özelliği tanımlanmıştır. Bu özellik, bir kategorinin ilişkili olduğu tüm ürünleri temsil eder.
    }
}

//6c7934bf-90d4-454f-83c7-baab40a2259a
//a435f15-136f-41c2-b703-094782bdea6d