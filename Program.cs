using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using Pgvector.EntityFrameworkCore;
using ProductCatalog.Api.Services;
using Npgsql;

/*
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

// ==========================================================
// KESİN ÇÖZÜM: CORS'u pipeline'ın EN BAŞINA koyuyoruz!
// ==========================================================
app.UseCors("AllowAdminPanel");

// Otomatik Migration ve Type Reloading
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        context.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS vector;");
        context.Database.Migrate();

        var conn = (NpgsqlConnection)context.Database.GetDbConnection();
        conn.Open();
        conn.ReloadTypes();
        conn.Close();

        Console.WriteLine(">>> Veritabanı tipleri başarıyla yeniden yüklendi.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(">>> MIGRATION/RELOAD HATASI: " + ex.Message);
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();