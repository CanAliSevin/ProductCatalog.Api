using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;

/*
 * Program.cs
 * -------------------------
 * Bu dosya ASP.NET Core uygulamasının giriş noktasıdır.
 *
 * Görevi:
 * 1- Uygulamanın ihtiyaç duyduğu servisleri kaydetmek.
 * 2- HTTP isteklerinin hangi sırayla işleneceğini belirlemek.
 * 3- Uygulamayı ayağa kaldırıp istek dinlemeye başlatmak.
 *
 * Kısaca:
 * "Uygulamanın kurulumu ve başlangıç dosyasıdır."
 */


// Builder oluşturulur.
// Builder, uygulama daha çalışmadan önce yapılacak tüm hazırlıkları yapar.
// (Servis ekleme, ayar okuma, Dependency Injection vb.)
var builder = WebApplication.CreateBuilder(args);



// =========================
// SERVİS KAYITLARI
// =========================
// builder.Services içine yazılan her şey,
// uygulama çalışmadan önce Dependency Injection (DI)
// container'ına kayıt edilir.



// Controller desteğini ekler.
// Eğer bunu yazmazsak ASP.NET Core Controller sınıflarını tanımaz.
//
// Örneğin:
// ProductsController
// CategoriesController
// DukkansController
//
// gibi sınıflar çalışmaz.
builder.Services.AddControllers();



// OpenAPI (Swagger) servislerini ekler.
//
// Bunun sayesinde API dokümantasyonu oluşur.
// Daha sonra Swagger ekranından GET, POST, PUT gibi istekler atabiliriz.
builder.Services.AddOpenApi();



// Entity Framework Core'u uygulamaya ekler.
//
// Burada ApplicationDbContext'in kullanılacağını söylüyoruz.
//
// options => Veritabanı ayarlarını temsil eder.
//
// UseNpgsql()
// => PostgreSQL kullanacağımızı belirtir.
//
// ConnectionString ise appsettings.json dosyasından okunur.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);



// =========================
// UYGULAMAYI OLUŞTUR
// =========================
//
// Artık tüm servisler eklendi.
//
// Build() çağrıldıktan sonra
// servis koleksiyonu ReadOnly olur.
//
// !!! Yani bundan sonra AddDbContext(), !!!!!!!
// AddControllers() gibi yeni servis eklenemez.
var app = builder.Build();



// =========================
// MIDDLEWARE YAPILANDIRMASI
// =========================
//
// Bundan sonraki bölüm,
// HTTP isteği geldiğinde hangi sırayla çalışacağını belirler.



// Eğer uygulama Development ortamındaysa
// OpenAPI endpointlerini oluştur.
//
// Böylece Swagger kullanılabilir.
//
// Production ortamında genellikle Swagger kapalı olur.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



// HTTP ile gelen istekleri otomatik olarak
// HTTPS'e yönlendir.
//
// Amaç:
// Güvenli bağlantı kullanılması.
app.UseHttpsRedirection();



// Controller'ları uygulamaya bağlar.
//
// Gelen istekler artık Controller'lardaki
// Action metodlarına yönlendirilir.
//
// Örneğin:
//
// GET /api/products
//
// isteği geldiğinde
//
// ProductsController -> GetProducts()
// metodu çalıştırılır.
app.MapControllers();



// =========================
// UYGULAMAYI BAŞLAT
// =========================
//
// Bu satırdan sonra uygulama çalışmaya başlar.
//
// Artık Kestrel web sunucusu ayağa kalkar
// ve belirlenen port üzerinden HTTP isteklerini dinlemeye başlar.
//
// Örneğin:
//
// https://localhost:5073
//
// adresine gelen istekler işlenmeye başlanır.
app.Run();