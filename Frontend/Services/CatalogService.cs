using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

// Service to communicate with Catalog microservice
public class CatalogService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public CatalogService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Services:CatalogService"]!);
    }

    // Get all products from catalog
    public async Task<List<Product>> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync("/api/products");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Product>();
        }

        return new List<Product>();
    }

    // Get single product by ID
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/products/{id}");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Product>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        return null;
    }
}
