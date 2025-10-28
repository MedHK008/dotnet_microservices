using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Models;
using Frontend.Services;

namespace Frontend.Pages;

public class CartModel : PageModel
{
    private readonly CartService _cartService;
    private readonly AuthService _authService;

    public CartModel(CartService cartService, AuthService authService)
    {
        _cartService = cartService;
        _authService = authService;
    }

    public List<CartItem> CartItems { get; set; } = new();
    public decimal Total { get; set; }

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
            // Token is invalid - clear cookies and redirect to login
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("UserEmail");
            _cartService.ClearCart();
            return RedirectToPage("/Login");
        }

        CartItems = _cartService.GetCartItems();
        Total = _cartService.GetCartTotal();

        return Page();
    }

    public IActionResult OnPostRemove(int productId)
    {
        // Verify user is authenticated before allowing cart modifications
        if (string.IsNullOrEmpty(Request.Cookies["AuthToken"]))
        {
            return RedirectToPage("/Login");
        }

        _cartService.RemoveFromCart(productId);
        return RedirectToPage();
    }

    public IActionResult OnPostUpdateQuantity(int productId, int quantity)
    {
        // Verify user is authenticated before allowing cart modifications
        if (string.IsNullOrEmpty(Request.Cookies["AuthToken"]))
        {
            return RedirectToPage("/Login");
        }

        _cartService.UpdateQuantity(productId, quantity);
        return RedirectToPage();
    }

    public IActionResult OnPostClear()
    {
        // Verify user is authenticated before allowing cart modifications
        if (string.IsNullOrEmpty(Request.Cookies["AuthToken"]))
        {
            return RedirectToPage("/Login");
        }

        _cartService.ClearCart();
        return RedirectToPage();
    }
}
