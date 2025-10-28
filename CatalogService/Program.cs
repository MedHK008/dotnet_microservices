using Microsoft.EntityFrameworkCore;
using CatalogService.Data;
using CatalogService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add database context with SQL Server
builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    // Suppress pending model changes warning
    options.ConfigureWarnings(warnings => 
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Add CORS to allow frontend to call this service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Apply migrations and seed database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowAll");
app.UseAuthorization();

// Get all products endpoint
app.MapGet("/api/products", async (CatalogDbContext db) =>
{
    var products = await db.Products.ToListAsync();
    return Results.Ok(products);
});

// Get single product by ID endpoint
app.MapGet("/api/products/{id}", async (int id, CatalogDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product != null ? Results.Ok(product) : Results.NotFound();
});

app.Run();
