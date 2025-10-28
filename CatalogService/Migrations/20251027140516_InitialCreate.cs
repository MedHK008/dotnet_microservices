using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "ImageUrl", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    // ...existing code...
                        { 1, new DateTime(2025, 10, 27, 14, 5, 14, 964, DateTimeKind.Utc).AddTicks(9173), "High-performance laptop with 16GB RAM and 512GB SSD", "https://ik.imagekit.io/MedHerak/products/laptop.jpeg?updatedAt=1761677974008", "Laptop Pro 15", 1299.99m, 50 },
                    // ...existing code...
                        { 2, new DateTime(2025, 10, 27, 14, 5, 14, 965, DateTimeKind.Utc).AddTicks(4811), "Ergonomic wireless mouse with precision tracking", "https://ik.imagekit.io/MedHerak/products/mouse.jpg?updatedAt=1761677974066", "Wireless Mouse", 29.99m, 200 },
                    // ...existing code...
                        { 3, new DateTime(2025, 10, 27, 14, 5, 14, 965, DateTimeKind.Utc).AddTicks(4829), "RGB mechanical keyboard with blue switches", "https://ik.imagekit.io/MedHerak/products/keyboard.jpg?updatedAt=1761677974064", "Mechanical Keyboard", 89.99m, 75 },
                    // ...existing code...
                        { 4, new DateTime(2025, 10, 27, 14, 5, 14, 965, DateTimeKind.Utc).AddTicks(4833), "27-inch 4K UHD monitor with HDR support", "https://ik.imagekit.io/MedHerak/products/monitor.png?updatedAt=1761677974086", "4K Monitor", 449.99m, 30 },
                    // ...existing code...
                        { 5, new DateTime(2025, 10, 27, 14, 5, 14, 965, DateTimeKind.Utc).AddTicks(4837), "7-in-1 USB-C hub with HDMI and ethernet ports", "https://ik.imagekit.io/MedHerak/products/hub.webp?updatedAt=1761677974386", "USB-C Hub", 49.99m, 150 },
                    // ...existing code...
                        { 6, new DateTime(2025, 10, 27, 14, 5, 14, 965, DateTimeKind.Utc).AddTicks(4840), "1080p HD webcam with built-in microphone", "https://ik.imagekit.io/MedHerak/products/cam.webp?updatedAt=1761677974061", "Webcam HD", 69.99m, 100 },
                    // ...existing code...
                        { 7, new DateTime(2025, 10, 27, 14, 5, 14, 965, DateTimeKind.Utc).AddTicks(4843), "Noise-cancelling over-ear headphones", "https://ik.imagekit.io/MedHerak/products/photo-1505740420928-5e560c06d30e_7tWlfCNAr.avif?updatedAt=1759220323847", "Headphones Pro", 199.99m, 60 },
                    // ...existing code...
                        { 8, new DateTime(2025, 10, 27, 14, 5, 14, 965, DateTimeKind.Utc).AddTicks(4846), "Portable external SSD with USB 3.2 Gen 2", "https://ik.imagekit.io/MedHerak/products/ssd.jpg?updatedAt=1761677973996", "External SSD 1TB", 129.99m, 80 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
