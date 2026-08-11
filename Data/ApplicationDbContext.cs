using System;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<Product> Products { get; set; }//bu, Product sınıfının veritabanında bir tablo olarak temsil edileceğini belirtir. Products özelliği, Product nesnelerinin veritabanında saklanmasını ve sorgulanmasını sağlar. Bu sayede, uygulama içinde ürünlerle ilgili işlemleri gerçekleştirebiliriz.
		public DbSet<Store> Stores { get; set; }
		public DbSet<Category> Categories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasPostgresExtension("vector");
		}

	}

}
