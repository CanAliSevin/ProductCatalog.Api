using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Api.Models
{
	public class Product
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();//dünyada benzersiz ID


		[Required]//bu, Name özelliğinin boş bırakılamayacağını belirtir. Yani, bir ürün oluşturulurken Name alanının mutlaka doldurulması gerekmektedir.
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;//ürünün adı boş bir string olarak başlatılmıştır.

		public string? Description { get; set; }

		[Column(TypeName = "decimal(10,2)")]//bu, Price özelliğinin veritabanında decimal türünde ve 10 basamaklı, 2 ondalık basamaklı olarak saklanacağını belirtir.
		public decimal Price { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		public string? ImageUrl { get; set; }


		public Guid StoreId { get; set; }//bu, Product sınıfının Store sınıfıyla olan ilişkisini temsil eder. Her ürünün bir dükkanla ilişkili olduğunu belirtir. StoreId özelliği, ürünün hangi dükkana ait olduğunu belirten bir yabancı anahtar (foreign key) olarak kullanılır. Bu sayede, bir ürünün hangi dükkana ait olduğunu kolayca bulabiliriz.
		public Store? store { get; set; }//bu, Product sınıfının Store sınıfıyla olan ilişkisini temsil eder. Her ürünün bir dükkanla ilişkili olduğunu belirtir. StoreId özelliği, ürünün hangi dükkana ait olduğunu belirten bir yabancı anahtar (foreign key) olarak kullanılır. Bu sayede, bir ürünün hangi dükkana ait olduğunu kolayca bulabiliriz.

		public Guid CategoryId { get; set; }
		public Category? Category { get; set; }//bu, Product sınıfının Category sınıfıyla olan ilişkisini temsil eder. Her ürünün bir kategoriyle ilişkili olduğunu belirtir. CategoryId özelliği, ürünün hangi kategoriye ait olduğunu belirten bir yabancı anahtar (foreign key) olarak kullanılır. Bu sayede, bir ürünün hangi kategoriye ait olduğunu kolayca bulabiliriz.





	}
}
