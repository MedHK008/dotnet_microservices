using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services;

// Service to manage shopping cart stored in cookies
public class CartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartCookieName = "ShoppingCart";

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Get cart items from cookie
    public List<CartItem> GetCartItems()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return new List<CartItem>();

        var cartCookie = context.Request.Cookies[CartCookieName];
        if (string.IsNullOrEmpty(cartCookie))
        {
            return new List<CartItem>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<CartItem>>(cartCookie) ?? new List<CartItem>();
        }
        catch
        {
            return new List<CartItem>();
        }
    }

    // Add item to cart
    public void AddToCart(Product product, int quantity = 1)
    {
        var cart = GetCartItems();
        // Check if item already exists in cart
        var existingItem = cart.FirstOrDefault(i => i.ProductId == product.Id);
        //  update quantity if so
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity
            });
        }

        SaveCart(cart);
    }

    // Remove item from cart
    public void RemoveFromCart(int productId)
    {
        var cart = GetCartItems();
        cart.RemoveAll(i => i.ProductId == productId);
        SaveCart(cart);
    }

    // Update item quantity
    public void UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCartItems();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
        }

        SaveCart(cart);
    }

    // Clear entire cart
    public void ClearCart()
    {
        // we should delete the cookie
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        context.Response.Cookies.Delete(CartCookieName);
        // SaveCart(new List<CartItem>());
    }

    // Get total cart value
    public decimal GetCartTotal()
    {
        return GetCartItems().Sum(i => i.Price * i.Quantity);
    }

    // Save cart to cookie
    private void SaveCart(List<CartItem> cart)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        var json = JsonSerializer.Serialize(cart);
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(7),
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax
        };

        context.Response.Cookies.Append(CartCookieName, json, cookieOptions);
    }
}
