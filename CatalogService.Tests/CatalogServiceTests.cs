using Xunit;
using Microsoft.EntityFrameworkCore;
using CatalogService.Data;
using CatalogService.Models;

namespace CatalogService.Tests;

public class CatalogServiceTests
{
    private readonly DbContextOptions<CatalogDbContext> _dbContextOptions;

    public CatalogServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private CatalogDbContext CreateContext()
    {
        return new CatalogDbContext(_dbContextOptions);
    }

    #region Add Product Tests

    [Fact]
    public void AddProduct_WithValidData_ShouldSucceed()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Laptop",
            Description = "High-performance laptop",
            Price = 999.99m,
            ImageUrl = "https://example.com/laptop.jpg",
            Stock = 10
        };

        // Act
        context.Products.Add(product);
        context.SaveChanges();

        // Assert
        Assert.Single(context.Products);
        var savedProduct = context.Products.First();
        Assert.Equal("Laptop", savedProduct.Name);
        Assert.Equal(999.99m, savedProduct.Price);
    }

    [Fact]
    public void AddProduct_ShouldSetCreatedAtTimestamp()
    {
        // Arrange
        var context = CreateContext();
        var beforeCreation = DateTime.UtcNow;
        var product = new Product
        {
            Name = "Monitor",
            Description = "4K Monitor",
            Price = 399.99m,
            ImageUrl = "https://example.com/monitor.jpg",
            Stock = 5
        };

        // Act
        context.Products.Add(product);
        context.SaveChanges();

        var afterCreation = DateTime.UtcNow;

        // Assert
        var savedProduct = context.Products.First();
        Assert.InRange(savedProduct.CreatedAt, beforeCreation, afterCreation);
    }

    [Theory]
    [InlineData("Mouse", "Wireless Mouse", 29.99, 50)]
    [InlineData("Keyboard", "Mechanical Keyboard", 89.99, 30)]
    [InlineData("Headphones", "Bluetooth Headphones", 149.99, 20)]
    public void AddProduct_WithVariousProducts_ShouldSucceed(string name, string description, decimal price, int stock)
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = name,
            Description = description,
            Price = price,
            ImageUrl = "https://example.com/product.jpg",
            Stock = stock
        };

        // Act
        context.Products.Add(product);
        context.SaveChanges();

        // Assert
        var savedProduct = context.Products.First();
        Assert.Equal(name, savedProduct.Name);
        Assert.Equal(price, savedProduct.Price);
        Assert.Equal(stock, savedProduct.Stock);
    }

    #endregion

    #region Retrieve Product Tests

    [Fact]
    public void GetAllProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var context = CreateContext();
        var products = new List<Product>
        {
            new Product { Name = "Product 1", Description = "Desc 1", Price = 10m, Stock = 5 },
            new Product { Name = "Product 2", Description = "Desc 2", Price = 20m, Stock = 10 },
            new Product { Name = "Product 3", Description = "Desc 3", Price = 30m, Stock = 15 }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Act
        var allProducts = context.Products.ToList();

        // Assert
        Assert.Equal(3, allProducts.Count);
    }

    [Fact]
    public void GetProductById_WithExistingId_ShouldReturnProduct()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 50m,
            Stock = 10
        };

        context.Products.Add(product);
        context.SaveChanges();
        var productId = product.Id;

        // Act
        var foundProduct = context.Products.Find(productId);

        // Assert
        Assert.NotNull(foundProduct);
        Assert.Equal("Test Product", foundProduct.Name);
        Assert.Equal(productId, foundProduct.Id);
    }

    [Fact]
    public void GetProductById_WithNonexistentId_ShouldReturnNull()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var product = context.Products.Find(999);

        // Assert
        Assert.Null(product);
    }

    [Fact]
    public void GetProductsByPrice_ShouldFilterCorrectly()
    {
        // Arrange
        var context = CreateContext();
        var products = new List<Product>
        {
            new Product { Name = "Cheap", Description = "Cheap product", Price = 10m, Stock = 10 },
            new Product { Name = "Medium", Description = "Medium product", Price = 50m, Stock = 10 },
            new Product { Name = "Expensive", Description = "Expensive product", Price = 200m, Stock = 10 }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Act
        var expensiveProducts = context.Products.Where(p => p.Price > 100).ToList();

        // Assert
        Assert.Single(expensiveProducts);
        Assert.Equal("Expensive", expensiveProducts.First().Name);
    }

    [Fact]
    public void GetProductsByName_ShouldFilterCorrectly()
    {
        // Arrange
        var context = CreateContext();
        var products = new List<Product>
        {
            new Product { Name = "Laptop Pro", Description = "High-end laptop", Price = 999m, Stock = 10 },
            new Product { Name = "Laptop Air", Description = "Lightweight laptop", Price = 799m, Stock = 10 },
            new Product { Name = "Desktop", Description = "Desktop computer", Price = 1299m, Stock = 5 }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Act
        var laptops = context.Products.Where(p => p.Name.Contains("Laptop")).ToList();

        // Assert
        Assert.Equal(2, laptops.Count);
    }

    #endregion

    #region Update Product Tests

    [Fact]
    public void UpdateProduct_ShouldUpdateSuccessfully()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Original Name",
            Description = "Original Description",
            Price = 50m,
            Stock = 10
        };

        context.Products.Add(product);
        context.SaveChanges();

        // Act
        product.Name = "Updated Name";
        product.Price = 75m;
        context.SaveChanges();

        // Assert
        var updatedProduct = context.Products.Find(product.Id);
        Assert.Equal("Updated Name", updatedProduct.Name);
        Assert.Equal(75m, updatedProduct.Price);
    }

    [Fact]
    public void UpdateProductStock_ShouldDecreaseStock()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Stock Test",
            Description = "Test",
            Price = 30m,
            Stock = 100
        };

        context.Products.Add(product);
        context.SaveChanges();

        // Act
        product.Stock -= 10;
        context.SaveChanges();

        // Assert
        var updatedProduct = context.Products.Find(product.Id);
        Assert.Equal(90, updatedProduct.Stock);
    }

    [Fact]
    public void UpdateProductStock_ShouldIncreaseStock()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Stock Test",
            Description = "Test",
            Price = 30m,
            Stock = 50
        };

        context.Products.Add(product);
        context.SaveChanges();

        // Act
        product.Stock += 25;
        context.SaveChanges();

        // Assert
        var updatedProduct = context.Products.Find(product.Id);
        Assert.Equal(75, updatedProduct.Stock);
    }

    [Fact]
    public void UpdateProduct_ShouldNotChangeCreatedAtTimestamp()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Test",
            Description = "Test",
            Price = 30m,
            Stock = 10
        };

        context.Products.Add(product);
        context.SaveChanges();
        var originalCreatedAt = product.CreatedAt;

        // Act
        System.Threading.Thread.Sleep(100); // Wait a bit
        product.Name = "Updated";
        context.SaveChanges();

        // Assert
        var updatedProduct = context.Products.Find(product.Id);
        Assert.Equal(originalCreatedAt, updatedProduct.CreatedAt);
    }

    #endregion

    #region Delete Product Tests

    [Fact]
    public void DeleteProduct_ShouldRemoveSuccessfully()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "To Delete",
            Description = "This will be deleted",
            Price = 10m,
            Stock = 5
        };

        context.Products.Add(product);
        context.SaveChanges();
        var productId = product.Id;

        // Act
        context.Products.Remove(product);
        context.SaveChanges();

        // Assert
        var deletedProduct = context.Products.Find(productId);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public void DeleteProduct_ShouldNotAffectOtherProducts()
    {
        // Arrange
        var context = CreateContext();
        var product1 = new Product { Name = "Product 1", Description = "Desc 1", Price = 10m, Stock = 5 };
        var product2 = new Product { Name = "Product 2", Description = "Desc 2", Price = 20m, Stock = 10 };
        var product3 = new Product { Name = "Product 3", Description = "Desc 3", Price = 30m, Stock = 15 };

        context.Products.AddRange(product1, product2, product3);
        context.SaveChanges();

        // Act
        context.Products.Remove(product2);
        context.SaveChanges();

        // Assert
        var remainingProducts = context.Products.ToList();
        Assert.Equal(2, remainingProducts.Count);
        Assert.Contains(remainingProducts, p => p.Name == "Product 1");
        Assert.Contains(remainingProducts, p => p.Name == "Product 3");
        Assert.DoesNotContain(remainingProducts, p => p.Name == "Product 2");
    }

    #endregion

    #region Stock Management Tests

    [Fact]
    public void GetLowStockProducts_ShouldReturnProductsWithLowStock()
    {
        // Arrange
        var context = CreateContext();
        var products = new List<Product>
        {
            new Product { Name = "Low Stock", Description = "Low stock product", Price = 50m, Stock = 2 },
            new Product { Name = "Medium Stock", Description = "Medium stock product", Price = 50m, Stock = 20 },
            new Product { Name = "High Stock", Description = "High stock product", Price = 50m, Stock = 100 }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Act
        var lowStockProducts = context.Products.Where(p => p.Stock < 10).ToList();

        // Assert
        Assert.Single(lowStockProducts);
        Assert.Equal("Low Stock", lowStockProducts.First().Name);
    }

    [Fact]
    public void GetOutOfStockProducts_ShouldReturnProducts()
    {
        // Arrange
        var context = CreateContext();
        var products = new List<Product>
        {
            new Product { Name = "Out of Stock", Description = "No stock", Price = 50m, Stock = 0 },
            new Product { Name = "In Stock", Description = "Has stock", Price = 50m, Stock = 10 }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Act
        var outOfStock = context.Products.Where(p => p.Stock == 0).ToList();

        // Assert
        Assert.Single(outOfStock);
        Assert.Equal("Out of Stock", outOfStock.First().Name);
    }

    [Fact]
    public void CanBuyProduct_WithSufficientStock_ShouldSucceed()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Buyable",
            Description = "Has enough stock",
            Price = 50m,
            Stock = 100
        };

        context.Products.Add(product);
        context.SaveChanges();

        // Act
        var canBuy = product.Stock >= 5;
        if (canBuy)
        {
            product.Stock -= 5;
            context.SaveChanges();
        }

        // Assert
        Assert.True(canBuy);
        Assert.Equal(95, product.Stock);
    }

    [Fact]
    public void CanBuyProduct_WithInsufficientStock_ShouldFail()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Low Stock Item",
            Description = "Not enough stock",
            Price = 50m,
            Stock = 2
        };

        context.Products.Add(product);
        context.SaveChanges();

        var requestedQuantity = 5;

        // Act
        var canBuy = product.Stock >= requestedQuantity;

        // Assert
        Assert.False(canBuy);
        Assert.Equal(2, product.Stock);
    }

    #endregion

    #region Product Properties Tests

    [Fact]
    public void Product_ShouldHaveValidProperties()
    {
        // Arrange & Act
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            ImageUrl = "https://example.com/image.jpg",
            Stock = 50
        };

        // Assert
        Assert.Equal("Test Product", product.Name);
        Assert.Equal("Test Description", product.Description);
        Assert.Equal(99.99m, product.Price);
        Assert.Equal("https://example.com/image.jpg", product.ImageUrl);
        Assert.Equal(50, product.Stock);
    }

    [Fact]
    public void Product_WithNegativePrice_ShouldStillSave()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Negative Price",
            Description = "Test",
            Price = -10m,
            Stock = 10
        };

        // Act
        context.Products.Add(product);
        context.SaveChanges();

        // Assert
        var savedProduct = context.Products.First();
        Assert.Equal(-10m, savedProduct.Price);
    }

    [Fact]
    public void Product_WithNegativeStock_ShouldStillSave()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Negative Stock",
            Description = "Test",
            Price = 50m,
            Stock = -5
        };

        // Act
        context.Products.Add(product);
        context.SaveChanges();

        // Assert
        var savedProduct = context.Products.First();
        Assert.Equal(-5, savedProduct.Stock);
    }

    [Fact]
    public void Product_WithVeryLargePrice_ShouldSave()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Expensive Product",
            Description = "Very expensive",
            Price = 999999999.99m,
            Stock = 1
        };

        // Act
        context.Products.Add(product);
        context.SaveChanges();

        // Assert
        var savedProduct = context.Products.First();
        Assert.Equal(999999999.99m, savedProduct.Price);
    }

    #endregion

    #region Multiple Operations Tests

    [Fact]
    public void ComplexScenario_AddUpdateDelete_ShouldWorkCorrectly()
    {
        // Arrange
        var context = CreateContext();
        var product = new Product
        {
            Name = "Complex Product",
            Description = "Test",
            Price = 50m,
            Stock = 100
        };

        // Act - Add
        context.Products.Add(product);
        context.SaveChanges();
        var addedId = product.Id;

        // Act - Update
        product.Price = 75m;
        product.Stock = 80;
        context.SaveChanges();

        // Act - Delete
        context.Products.Remove(product);
        context.SaveChanges();

        // Assert
        var finalProduct = context.Products.Find(addedId);
        Assert.Null(finalProduct);
    }

    [Fact]
    public void BulkOperations_AddMultipleProducts_ShouldSucceed()
    {
        // Arrange
        var context = CreateContext();
        var products = new List<Product>();
        for (int i = 0; i < 100; i++)
        {
            products.Add(new Product
            {
                Name = $"Product {i}",
                Description = $"Description {i}",
                Price = i * 10m,
                Stock = i * 5
            });
        }

        // Act
        context.Products.AddRange(products);
        context.SaveChanges();

        // Assert
        Assert.Equal(100, context.Products.Count());
    }

    #endregion
}
