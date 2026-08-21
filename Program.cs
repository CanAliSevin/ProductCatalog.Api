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

// OpenAPI (Swagger) servislerini ekler.
builder.Services.AddOpenApi();

// VERİTABANI VE VECTOR AYARLARI
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Npgsql'in vektör verisini okuyabilmesi için zorunlu global eşleştirme:
#pragma warning disable Npgsql0001
NpgsqlConnection.GlobalTypeMapper.UseVector();
#pragma warning restore Npgsql0001

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.UseVector();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(dataSource, o => o.UseVector())
);
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