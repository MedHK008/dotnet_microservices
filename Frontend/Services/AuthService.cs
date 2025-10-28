using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

// Service to communicate with Auth microservice
public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    // Constructor
    public AuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["Services:AuthService"]!);
    }

    // Register a new user
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/auth/register", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Registration successful, processing response...");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        Console.WriteLine("Registration failed.");
        return null;
    }

    // Login existing user
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Login successful, processing response...");
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        Console.WriteLine("Login failed.");

        return null;
    }

    // token validation
    public async Task<bool> ValidateTokenAsync(string token)
    {
        var payload = JsonSerializer.Serialize(new { Token = token });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        // Endpoint to validate token
        var response = await _httpClient.PostAsync("/api/auth/validate", content);
        return response.IsSuccessStatusCode;
    }
}
