using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using Pgvector.EntityFrameworkCore;
using ProductCatalog.Api.Services;
using Npgsql;

/*
 * Program.cs
 * -------------------------
 * Bu dosya ASP.NET Core uygulamasının giriş noktasıdır.
 */

var builder = WebApplication.CreateBuilder(args);

// =========================
// SERVİS KAYITLARI
// =========================

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminPanel", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

// =========================
// VERİTABANI VE VECTOR AYARLARI
// =========================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.UseVector(); // Vektör desteğini Npgsql seviyesinde açıyoruz
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(dataSource, o => o.UseVector()) // EF Core seviyesinde açıyoruz
);

builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
});
builder.Services.AddScoped<ProductSearchService>();

// =========================
// UYGULAMAYI OLUŞTUR
// =========================
var app = builder.Build();

// Otomatik Migration ve Type Reloading (Vektör Hatasının Kesin Çözümü)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        // 1. Vektör eklentisini garanti altına al
        context.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS vector;");

        // 2. Tabloları oluştur
        context.Database.Migrate();

        // 3. EN ÖNEMLİ ADIM: Npgsql'in tip önbelleğini zorla temizle ve PostgreSQL'den yeniden çek!
        var conn = (NpgsqlConnection)context.Database.GetDbConnection();
        conn.Open();
        conn.ReloadTypes(); // DataTypeName '-.-' hatasını tam olarak burası çözecek
        conn.Close();

        Console.WriteLine(">>> Veritabanı tipleri başarıyla yeniden yüklendi.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(">>> MIGRATION/RELOAD HATASI: " + ex.Message);
    }
}

app.UseCors("AllowAdminPanel");

// Swagger'ın hem Development hem Production'da açılmasını sağlar
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();