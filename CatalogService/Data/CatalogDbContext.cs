using Microsoft.EntityFrameworkCore;
using CatalogService.Models;

namespace CatalogService.Data;

// Database context for Catalog service
public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    // Seed initial product data
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure decimal precision for Price property
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop Pro 15",
                Description = "High-performance laptop with 16GB RAM and 512GB SSD",
                Price = 1299.99m,
                ImageUrl = "https://ik.imagekit.io/MedHerak/products/laptop.jpeg?updatedAt=1761677974008",
                Stock = 50
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse with precision tracking",
                Price = 29.99m,
                ImageUrl = "https://ik.imagekit.io/MedHerak/products/mouse.jpg?updatedAt=1761677974066",
                Stock = 200
            },
            new Product
            {
                Id = 3,
                Name = "Mechanical Keyboard",
                Description = "RGB mechanical keyboard with blue switches",
                Price = 89.99m,
                ImageUrl = "https://ik.imagekit.io/MedHerak/products/keyboard.jpg?updatedAt=1761677974064",
                Stock = 75
            },
            new Product
            {
                Id = 4,
                Name = "4K Monitor",
                Description = "27-inch 4K UHD monitor with HDR support",
                Price = 449.99m,
                ImageUrl = "https://ik.imagekit.io/MedHerak/products/monitor.png?updatedAt=1761677974086",
                Stock = 30
            },
            new Product
            {
                Id = 5,
                Name = "USB-C Hub",
                Description = "7-in-1 USB-C hub with HDMI and ethernet ports",
                Price = 49.99m,
                ImageUrl = "https://ik.imagekit.io/MedHerak/products/hub.webp?updatedAt=1761677974386",
                Stock = 150
            },
            new Product
            {
                Id = 6,
                Name = "Webcam HD",
                Description = "1080p HD webcam with built-in microphone",
                Price = 69.99m,
                ImageUrl = "https://ik.imagekit.io/MedHerak/products/cam.webp?updatedAt=1761677974061",
                Stock = 100
            },
            new Product
            {
                Id = 7,
                Name = "Headphones Pro",
                Description = "Noise-cancelling over-ear headphones",
                Price = 199.99m,
                ImageUrl = "https://ik.imagekit.io/MedHerak/products/photo-1505740420928-5e560c06d30e_7tWlfCNAr.avif?updatedAt=1759220323847",
                Stock = 60
            },
            new Product
            {
                Id = 8,
                Name = "External SSD 1TB",
                Description = "Portable external SSD with USB 3.2 Gen 2",
                Price = 129.99m,
                ImageUrl = "https://ik.imagekit.io/MedHerak/products/ssd.jpg?updatedAt=1761677973996",
                Stock = 80
            }
        );
    }
}
