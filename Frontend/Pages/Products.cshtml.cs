using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Models;
using Frontend.Services;

namespace Frontend.Pages;

public class ProductsModel : PageModel
{
    private readonly CatalogService _catalogService;
    private readonly CartService _cartService;
    private readonly AuthService _authService;

    public ProductsModel(CatalogService catalogService, CartService cartService, AuthService authService)
    {
        _catalogService = catalogService;
        _cartService = cartService;
        _authService = authService;
    }

    public List<Product> Products { get; set; } = new();
    public string? UserEmail { get; set; }
    public int CartItemCount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is authenticated
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }

        // Validate token with auth service
        var isValid = await _authService.ValidateTokenAsync(token);
        if (!isValid)
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("UserEmail");
            return RedirectToPage("/Login");
        }

        UserEmail = Request.Cookies["UserEmail"];

        // Get products from catalog service
        Products = await _catalogService.GetProductsAsync();

        // Get cart count
        CartItemCount = _cartService.GetCartItems().Sum(i => i.Quantity);

        return Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId)
    {
        // Get product details
        var product = await _catalogService.GetProductByIdAsync(productId);
        if (product != null)
        {
            _cartService.AddToCart(product);
        }

        return RedirectToPage();
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("UserEmail");
        _cartService.ClearCart();
        return RedirectToPage("/Login");
    }
}
