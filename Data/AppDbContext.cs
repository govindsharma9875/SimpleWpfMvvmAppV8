using Microsoft.EntityFrameworkCore;
using SimpleWpfToMvcApp.Web.Models;

namespace SimpleWpfToMvcApp.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity with modern fluent API
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Users_Email");

            // Seed data
            entity.HasData(
                new User { Id = 1, Name = "John Doe", Email = "john.doe@example.com" },
                new User { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com" }
            );
        });

        // Configure Product entity with modern fluent API
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ImagePath).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasIndex(e => e.SKU).IsUnique().HasDatabaseName("IX_Products_SKU");

            // Seed data
            entity.HasData(
                new Product 
                { 
                    Id = 1, 
                    Name = "Sample Product 1", 
                    Description = "This is a sample product for demonstration", 
                    Price = 29.99m, 
                    SKU = "SAMPLE-001", 
                    IsActive = true
                },
                new Product 
                { 
                    Id = 2, 
                    Name = "Sample Product 2", 
                    Description = "Another sample product", 
                    Price = 49.99m, 
                    SKU = "SAMPLE-002", 
                    IsActive = true
                }
            );
        });
    }
}
