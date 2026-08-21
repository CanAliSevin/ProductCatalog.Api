using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using Pgvector.EntityFrameworkCore;
using ProductCatalog.Api.Services;

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

// OpenAPI (Swagger) servislerini ekler.
builder.Services.AddOpenApi();

// Entity Framework Core & PostgreSQL (pgvector)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseVector()
    )
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

// Otomatik Migration (Tabloları veritabanında oluşturur)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.UseCors("AllowAdminPanel");

// Swagger'ın hem Development hem Production'da açılmasını sağlar
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();